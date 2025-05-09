using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueTyper : MonoBehaviour
{
    public GameObject dialoguePanel; // Drag your DialogueBox here
    public TextMeshProUGUI dialogueText; // Drag your DialogueText here
    public AudioSource typingSound; // Drag your AudioSource here
    public float typingSpeed = 0.02f;
    public float delayBetweenLines = 2f; // Time between dialogue lines

    [TextArea(3, 10)]
    public string[] startDialogue; // Set this in the Inspector
    private int currentLine = 0;
    private Coroutine typingCoroutine;

    private bool isTyping = false;
    private bool canAdvance = false;

    public bool isDialogueActive = false;



    void Start()
    {
        dialoguePanel.SetActive(false); // Hide at first
        StartCoroutine(StartDialogueWithDelay(1f)); // Wait 1 second before showing text
    }

    IEnumerator StartDialogueWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (startDialogue.Length > 0)
        {
            dialoguePanel.SetActive(true); // Show after delay
            ShowDialogue(startDialogue[currentLine]);
        }
    }

    public void ShowDialogue(string message)
    {
        if (dialoguePanel != null && !dialoguePanel.activeSelf)
            dialoguePanel.SetActive(true);

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    public void PlayDialogue(string[] lines)
    {
        isDialogueActive = true;
        StopAllCoroutines();
        dialoguePanel.SetActive(true);
        currentLine = 0;
        startDialogue = lines;
        ShowDialogue(startDialogue[currentLine]);
    }



    IEnumerator TypeText(string message)
    {
        isTyping = true;
        canAdvance = false;
        dialogueText.text = "";

        float soundTimer = 0f;
        float soundInterval = 0.07f; // adjust for slower/faster sound effect rate

        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter;

            if (typingSound != null && !char.IsWhiteSpace(letter))
            {
                soundTimer -= typingSpeed;
                if (soundTimer <= 0f)
                {
                    typingSound.PlayOneShot(typingSound.clip);
                    soundTimer = soundInterval;
                }
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        canAdvance = true;

        // wait for time *or* player input to advance
        float timer = 0f;
        while (timer < delayBetweenLines && !Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.E))
        {
            timer += Time.deltaTime;
            yield return null;
        }

        NextDialogue();
    }


    public void NextDialogue()
    {
        if (currentLine < startDialogue.Length - 1)
        {
            currentLine++;
            ShowDialogue(startDialogue[currentLine]);
        }
        else
        {
            StartCoroutine(HideAfterDelay(2f)); // Hide after last line
        }
    }

    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
    }
}
