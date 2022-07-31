using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class DialogueController : MonoBehaviour
{
    /// <summary>
    /// TMPro Dialogue box to render text to
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI DialogueTextBox;

    /// <summary>
    /// The delay in seconds between character rendering
    /// </summary>
    [SerializeField]
    private float characterWriteDelay = 0.03f;

    /// <summary>
    /// The maximum dialogue length - used to make sure dialogue fits, there's probably a better way to do this
    /// </summary>
    [SerializeField]
    private int maxDialogueItemLength = 170;

    public Action DialogueStarted;
    public Action AllDialogueCompleted;

    private CharacterPlayerControls controls;

    private bool isDialogueItemFastCompleteRequested = false;
    private bool isNextDialogueItemRequested = false;

    private WaitForSeconds characterWriteDelayWait;
    private WaitUntil waitUntilNextDialogueItemRequested;

    private Button button;
    private EventSystem eventSystem;

    private void Awake()
    {
        Utility.LogErrorIfNull(DialogueTextBox, nameof(DialogueTextBox), "DialogueController has no textbox to control");

        button = GetComponent<Button>();
        Utility.LogErrorIfNull(button, nameof(button), "No dialogue button");

        eventSystem = GameObject.Find("EventSystem")?.GetComponent<EventSystem>();
        Utility.LogErrorIfNull(eventSystem, nameof(eventSystem));

        characterWriteDelayWait = new WaitForSeconds(characterWriteDelay);
        waitUntilNextDialogueItemRequested = new WaitUntil(() => isNextDialogueItemRequested);

        controls = new CharacterPlayerControls();
        controls.Gameplay.Disable();

        DialogueStarted += OnDialogueStarted;
        AllDialogueCompleted += OnAllDialogueCompleted;
    }

    private void OnDestroy()
    {
        DialogueStarted -= OnDialogueStarted;
        AllDialogueCompleted -= OnAllDialogueCompleted;
    }

    public void BeginDialogue(List<string> dialogueList)
    {
        dialogueList.ForEach(di => ValidateDialogueItem(di));
        StartCoroutine(WriteDialogue(dialogueList));
    }

    private IEnumerator WriteDialogue(List<string> dialogueList)
    {
        DialogueStarted.Invoke();

        foreach (string dialogueItem in dialogueList)
        {
            foreach (char character in dialogueItem.ToCharArray())
            {
                if (isDialogueItemFastCompleteRequested)
                {
                    DialogueTextBox.text = dialogueItem;
                    OnRespondedToProgressDialogueAction();
                    break;
                }

                DialogueTextBox.text += character;
                yield return characterWriteDelayWait;
            }

            yield return waitUntilNextDialogueItemRequested;
            OnRespondedToProgressDialogueAction();
            DialogueTextBox.text = string.Empty;
        }

        AllDialogueCompleted.Invoke();
    }

    private void OnDialogueStarted()
    {
        controls.DialogueControl.Enable();
        controls.DialogueControl.ProgressDialogueAction.performed += OnControlDeviceInput;
        Utility.AddButtonCallback(button, () => OnProgressDialogueAction());
    }

    private void OnAllDialogueCompleted()
    {
        controls.DialogueControl.Disable();
        controls.DialogueControl.ProgressDialogueAction.performed -= OnControlDeviceInput;
        Utility.ClearButtonAllCallbacks(button);
    }

    private void OnControlDeviceInput(CallbackContext context)
    {
        // lol, paulatwarp's answer - https://answers.unity.com/questions/945299/how-to-trigger-a-button-click-from-script.html
        // manually doing onClick.Invoke() doesn't do the darken-button-on-press effect, but this does
        ExecuteEvents.Execute(button.gameObject, eventData: new BaseEventData(eventSystem), ExecuteEvents.submitHandler);
    }

    private void OnProgressDialogueAction()
    {
        isDialogueItemFastCompleteRequested = true;
        isNextDialogueItemRequested = true;
    }

    private void OnRespondedToProgressDialogueAction()
    {
        isDialogueItemFastCompleteRequested = false;
        isNextDialogueItemRequested = false;
    }

    private void ValidateDialogueItem(string dialogueItem)
    {
        if (dialogueItem.Length <= maxDialogueItemLength)
            return;

        Debug.LogError($"Dialogue item is too long for panel bounds. maxlen={maxDialogueItemLength}, itemlen={dialogueItem.Length}, item='{dialogueItem}'");
    }
}
