using UnityEngine;
using TMPro;
using System.Data.Common;

public class CabinetDoorInteract : MonoBehaviour
{
    public Animator animator;
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string promptMessage = "Press E";

    public GameObject lockpickObject; // this is the lockpick hidden inside

    private bool isPlayerInRange = false;
    private bool isOpen = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (ChoreManager.instance.canPickUpKey)
            {
                OpenCabinet();
            }
            else
            {
                // say nothing or "I have no reason to open this right now"
                DialogueTyper typer = FindObjectOfType<DialogueTyper>();
                typer.PlayDialogue(new string[] { "Beautiful Cabinet." });
            }
        }
    }

    void OpenCabinet()
    {
        isOpen = true;
        animator.SetBool("isOpen", true);
        promptUI.SetActive(false);

        // Show the lockpick when cabinet is opened
        if (lockpickObject != null)
            lockpickObject.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isOpen) {
                isPlayerInRange = true;
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
            promptUI.SetActive(false);
        }
    }
}
