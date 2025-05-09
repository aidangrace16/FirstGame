using UnityEngine;
using TMPro;
using System.Collections;

public class SimpleInteract : MonoBehaviour
{
    [Header("Prompt")]
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string promptMessage = "Press E to read";

    private bool isPlayerInRange = false;

    public DialogueTyper dialogueTyper;
    [TextArea(3, 10)]
    public string[] noteDialogueLines;

    private bool hasTriggeredNoteDialogue = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (dialogueTyper != null)
            {
                if (!hasTriggeredNoteDialogue)
                {
                    hasTriggeredNoteDialogue = true;
                    dialogueTyper.PlayDialogue(noteDialogueLines);
                    StartCoroutine(ShowPostNoteLine());
                }
                else
                {
                    dialogueTyper.PlayDialogue(noteDialogueLines);
                }
            }
        }
    }

    IEnumerator ShowPostNoteLine()
    {
        while (dialogueTyper.isDialogueActive)
        {
            yield return null;
        }

        dialogueTyper.PlayDialogue(new string[] { "Anyways, I should get stuff done." });

        yield return new WaitForSeconds(1f);
        ObjectiveManager.instance.ShowObjective("\ndo the dishes.");
        ChoreManager.instance.noteHasBeenRead = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (promptUI != null && promptText != null)
            {
                promptText.text = promptMessage;
                promptUI.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }
}
