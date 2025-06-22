using UnityEngine;
using TMPro;

public class PickupAndPlace : MonoBehaviour
{
    public GameObject promptText;
    public TextMeshProUGUI promptTMP;
    public GameObject heldObject;
    public GameObject finalPlacedObject;

    private bool isPlayerInPickupZone = false;
    private bool isPlayerInDropZone = false;

    void Update()
    {
        if (isPlayerInPickupZone && !ChoreManager.instance.holdingLaundry && Input.GetKeyDown(KeyCode.E))
        {
            ChoreManager.instance.holdingLaundry = true;
            heldObject.SetActive(true);
            gameObject.SetActive(false); // hide pickup pile
            promptText.SetActive(false);

            DialogueTyper typer = FindObjectOfType<DialogueTyper>();
            typer.PlayDialogue(new string[] { "Ahh yes, let me place this on my bed." });
        }

        if (isPlayerInDropZone && ChoreManager.instance.holdingLaundry && Input.GetKeyDown(KeyCode.E))
        {
            ChoreManager.instance.holdingLaundry = false;
            ChoreManager.instance.laundryPlaced = true;
            heldObject.SetActive(false);
            finalPlacedObject.SetActive(true); // laundry on bed
            promptText.SetActive(false);

            DialogueTyper typer = FindObjectOfType<DialogueTyper>();
            typer.PlayDialogue(new string[] { "cool. now what?" });

            // NEXT OBJECTIVE
            ObjectiveManager.instance.ShowObjective("?");
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (gameObject.name == "LaundryPile")
        {
            if (ChoreManager.instance.dishesDone && !ChoreManager.instance.laundryPlaced)
            {
                isPlayerInPickupZone = true;
                promptTMP.text = "Press E to pick up laundry";
                promptText.SetActive(true);
            }
        }

        else if (gameObject.name == "LaundryDropZone")
        {
            isPlayerInDropZone = true;

            if (ChoreManager.instance.holdingLaundry && !ChoreManager.instance.laundryPlaced)
            {
                promptTMP.text = "Press E to place laundry";
                promptText.SetActive(true);
            }
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (gameObject.name == "LaundryPile")
            isPlayerInPickupZone = false;
        else if (gameObject.name == "LaundryDropZone")
            isPlayerInDropZone = false;

        promptText.SetActive(false);
    }
}
