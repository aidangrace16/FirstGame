using System.Collections;
using UnityEngine;
using TMPro;

public class SimpleInteractV2 : MonoBehaviour
{
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string promptMessage = "Press E";
    public string[] dialogueLines;

    public float interactDistance = 3f;
    public float sphereCastRadius = 0.4f;
    public bool requireLightsOff = false;

    private bool isPlayerInRange = false;
    private bool hasInteracted = false;

    void Update()
    {
        // Require the note to have been read first
        if (!ChoreManager.instance.noteHasBeenRead)
        {
            if (isPlayerInRange)
            {
                isPlayerInRange = false;
                if (promptUI != null)
                    promptUI.SetActive(false);
            }
            return;
        }

        if (hasInteracted)
            return;

        // If requireLightsOff is true, only allow interaction when ambient light is low
        if (requireLightsOff && RenderSettings.ambientLight.maxColorComponent > 0.05f)
        {
            if (isPlayerInRange)
            {
                isPlayerInRange = false;
                if (promptUI != null)
                    promptUI.SetActive(false);
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
                DialogueTyper typer = FindObjectOfType<DialogueTyper>();
                if (typer != null && dialogueLines != null && dialogueLines.Length > 0)
                {
                    typer.PlayDialogue(dialogueLines);
                }
                hasInteracted = true;
                // Only hide the prompt if the player is looking at this object
                if (promptUI != null && promptUI.activeSelf)
                    promptUI.SetActive(false);
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
