using UnityEngine;
using TMPro;

public class SimpleInteract : MonoBehaviour
{
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string promptMessage = "Press E";

    public GameObject BGPanel; // Assign your BGPanel (note UI) here
    public GameObject closePromptUI; // Assign a UI object for the close prompt
    public TextMeshProUGUI closePromptText;

    public float interactDistance = 3f;
    public float sphereCastRadius = 0.4f;

    private bool isPlayerInRange = false;
    private bool noteIsOpen = false;

    [Header("Player Control")]
    public MonoBehaviour playerMovementScript; // Assign your movement script here
    public MonoBehaviour playerLookScript;     // Assign your look script here

    public DialogueTyper dialogueTyper; // Assign in inspector

    private bool hasPlayedPostNoteDialogue = false;
    private float noteOpenTime = 0f;

    [Header("Audio")]
    public AudioSource audioSource; // Assign in inspector
    public AudioClip openPaperSound;
    public AudioClip closePaperSound;

    void Update()
    {
        if (noteIsOpen)
        {
            if (closePromptUI != null && closePromptText != null && !closePromptUI.activeSelf)
            {
                closePromptUI.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                CloseNote();
            }
            return;
        }

        if (IsPlayerLookingAtObject())
        {
            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
                if (promptUI != null && promptText != null)
                {
                    promptText.text = promptMessage;
                    promptUI.SetActive(true);
                }
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenNote();
            }
        }
        else
        {
            if (isPlayerInRange)
            {
                isPlayerInRange = false;
                if (promptUI != null)
                    promptUI.SetActive(false);
            }
        }
    }

    void OpenNote()
    {
        noteIsOpen = true;
        noteOpenTime = Time.time;
        if (BGPanel != null)
            BGPanel.SetActive(true);
        if (promptUI != null && promptUI.activeSelf)
            promptUI.SetActive(false);
        if (closePromptUI != null && closePromptText != null)
        {
            closePromptUI.SetActive(true);
        }
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;
        if (playerLookScript != null)
            playerLookScript.enabled = false;
        // Play open paper sound
        if (audioSource != null && openPaperSound != null)
            audioSource.PlayOneShot(openPaperSound);
    }

    void CloseNote()
    {
        noteIsOpen = false;
        if (BGPanel != null)
            BGPanel.SetActive(false);
        if (closePromptUI != null)
            closePromptUI.SetActive(false);
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;
        if (playerLookScript != null)
            playerLookScript.enabled = true;

        // Play close paper sound
        if (audioSource != null && closePaperSound != null)
            audioSource.PlayOneShot(closePaperSound);

        // Play post-note dialogue only once
        if (!hasPlayedPostNoteDialogue && dialogueTyper != null)
        {
            hasPlayedPostNoteDialogue = true;
            if (Time.time - noteOpenTime < 1f)
            {
                dialogueTyper.PlayDialogue(new string[] { "I'm not reading this shit bruh 😂" });
            }
            else
            {
                dialogueTyper.PlayDialogue(new string[] { "Damn I hate that guy...", "Anyways, I should get stuff done." });
            }
            ObjectiveManager.instance.ShowObjective("\nDo the dishes");
            ChoreManager.instance.canPickUpKey = false; // (optional: ensure key can't be picked up yet)
            ChoreManager.instance.noteHasBeenRead = true;
            // Enable the dishes chore
            ChoreManager.instance.dishesDone = false; // Mark as not done (if needed)
            // If you have a flag to enable the dishes chore, set it here
            // e.g. ChoreManager.instance.canDoDishes = true;
        }

        promptUI.SetActive(true);
    }

    bool IsPlayerLookingAtObject()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, sphereCastRadius, out hit, interactDistance))
        {
            if (hit.collider == GetComponent<Collider>())
                return true;
        }
        return false;
    }
}
