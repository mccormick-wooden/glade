using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

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

    private List<string> dialogueCollection;

    public Action DialogueCompleted;

    //private bool isActivelyWritingDialogueItem;
    //private bool isInterDialogueItemDelay;

    private void Awake()
    {
        Utility.LogErrorIfNull(DialogueTextBox, nameof(DialogueTextBox), "DialogueController has no textbox to control");
        characterWriteDelay = characterWriteDelayDefaultState;
        interDialogueItemDelay = interDialogueItemDelayDefaultState;
    }

    public void BeginDialogue(IEnumerable<string> dialogueCollection)
    {
        this.dialogueCollection = new List<string>(dialogueCollection);
        StartCoroutine(WriteDialogue());
    }

    //// Update is called once per frame
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space)) // TODO - proper controller inputs
    //    {
    //        NextSentence();
    //    }
    //}

    //private void NextSentence()
    //{
    //    DialogueTextBox.text = string.Empty;
    //    //StartCoroutine(WriteSentence()); 
    //}


    private IEnumerator WriteDialogue()
    {    
        foreach (string dialogueItem in dialogueCollection)
        {
            //isInterDialogueItemDelay = false;

            foreach (char character in dialogueItem.ToCharArray())
            {
                //isActivelyWritingDialogueItem = true;

                DialogueTextBox.text += character;
                yield return new WaitForSeconds(characterWriteDelay);
            }

            //isActivelyWritingDialogueItem = false;
            //isInterDialogueItemDelay = true;

            yield return new WaitForSeconds(interDialogueItemDelay);
            DialogueTextBox.text = string.Empty;
        }

        //isActivelyWritingDialogueItem = false;
        //isInterDialogueItemDelay = false;
        DialogueCompleted?.Invoke();
    }
}
