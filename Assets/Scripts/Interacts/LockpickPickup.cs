using UnityEngine;
using TMPro;

public class LockpickPickup : MonoBehaviour
{
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string promptMessage = "Press E to pick up lockpick";

    private bool isPlayerInRange = false;

    public GameObject heldLockpick; // assign in inspector


    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Player picks up lockpick
            DialogueTyper typer = FindObjectOfType<DialogueTyper>();
            typer.PlayDialogue(new string[] { "This should work on the door..." });

            // Update objective
            ObjectiveManager.instance.ShowObjective("Unlock your roommate's door.");

            // You could also set a flag here like:
            ChoreManager.instance.hasLockpick = true;

            heldLockpick.SetActive(true);
            // Destroy the lockpick object
            Destroy(gameObject);

            // Hide prompt
            promptUI.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && ChoreManager.instance.canPickUpKey)
        {
            isPlayerInRange = true;
            promptText.text = promptMessage;
            promptUI.SetActive(true);
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
