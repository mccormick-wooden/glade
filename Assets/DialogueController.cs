using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    private float characterWriteDelayDefaultState = 0.03f;
    private float characterWriteDelay;

    /// <summary>
    /// The delay in seconds between when a dialogue item ends and the next dialogue item starts (or the end event)
    /// </summary>
    [SerializeField]
    private float interDialogueItemDelayDefaultState = 3f;
    private float interDialogueItemDelay;

    [SerializeField]
    private int maxDialogueItemLength = 170;

    public Action DialogueCompleted;

    private void Awake()
    {
        Utility.LogErrorIfNull(DialogueTextBox, nameof(DialogueTextBox), "DialogueController has no textbox to control");
        characterWriteDelay = characterWriteDelayDefaultState;
        interDialogueItemDelay = interDialogueItemDelayDefaultState;
    }

    private void Update()
    {
        // TODO: add user control of dialogue progression
    }

    public void BeginDialogue(List<string> dialogueList)
    {
        dialogueList.ForEach(di => ValidateDialogueItem(di));
        StartCoroutine(WriteDialogue(dialogueList));
    }

    private IEnumerator WriteDialogue(List<string> dialogueList)
    {    
        foreach (string dialogueItem in dialogueList)
        {
            foreach (char character in dialogueItem.ToCharArray())
            {
                DialogueTextBox.text += character;
                yield return new WaitForSeconds(characterWriteDelay);
            }

            yield return new WaitForSeconds(interDialogueItemDelay);
            DialogueTextBox.text = string.Empty;
        }

        DialogueCompleted?.Invoke();
    }
    
    private void ValidateDialogueItem(string dialogueItem)
    {
        if (dialogueItem.Length <= maxDialogueItemLength)
            return;

        Debug.LogError($"Dialogue item is too long for panel bounds. maxlen={maxDialogueItemLength}, itemlen={dialogueItem.Length}, item='{dialogueItem}'");
    }
}
