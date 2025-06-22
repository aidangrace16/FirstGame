using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HoldInteraction : MonoBehaviour
{
    public Image chargeBarFill;
    public GameObject chargePanel;
    public float holdTime = 2f;
    public string interactionPrompt = "Hold E to do dishes";
    public TextMeshProUGUI promptText;

    [Header("Sounds")]
    public AudioSource scareSound;
    public AudioClip knockClip;

    [Header("Dishes to enable/disable")]
    public GameObject dirtyDishes;
    public GameObject cleanDishes;

    private bool quietKnockPlayed = false;
    private bool midKnockTriggered = false;

    private float holdTimer = 0f;
    private bool taskDone = false;

    public float interactDistance = 3f;
    public float sphereCastRadius = 0.4f;

    private bool isPlayerLooking = false;

    void Update()
    {
        if (taskDone || !ChoreManager.instance.noteHasBeenRead)
            return;

        isPlayerLooking = IsPlayerLookingAtObject();

        if (isPlayerLooking)
        {
            if (promptText != null && !promptText.gameObject.activeSelf && !ChoreManager.instance.dishesDone)
            {
                promptText.text = interactionPrompt;
                promptText.gameObject.SetActive(true);
            }

            if (Input.GetKey(KeyCode.E) && !ChoreManager.instance.dishesDone)
            {
                if (!chargePanel.activeSelf)
                    chargePanel.SetActive(true);

                holdTimer += Time.deltaTime;
                chargeBarFill.fillAmount = holdTimer / holdTime;

                if (!quietKnockPlayed && holdTimer >= holdTime / 4f)
                {
                    quietKnockPlayed = true;

                    if (scareSound != null && knockClip != null)
                    {
                        scareSound.volume = 0.3f;
                        StartCoroutine(PlayKnockingSequence(0.5f));
                    }
                }

                if (!midKnockTriggered && holdTimer >= holdTime / 1.5f)
                {
                    midKnockTriggered = true;
                    StartCoroutine(PlayKnockingSequence(0.8f));
                }

                if (holdTimer >= holdTime)
                {
                    CompleteTask();
                }
            }
            else
            {
                if (chargePanel.activeSelf)
                    chargePanel.SetActive(false);
            }
        }
        else
        {
            // Only hide promptText if it is showing this object's interaction prompt
            if (promptText != null && promptText.gameObject.activeSelf && promptText.text == interactionPrompt)
                promptText.gameObject.SetActive(false);
            if (chargePanel.activeSelf)
                chargePanel.SetActive(false);
        }
    }

    void CompleteTask()
    {
        taskDone = true;
        dirtyDishes.SetActive(false);
        cleanDishes.SetActive(true);

        ChoreManager.instance.dishesDone = true;
        chargePanel.SetActive(false);
        DialogueTyper typer = FindObjectOfType<DialogueTyper>();
        typer.PlayDialogue(new string[] { "Dishes done.\n\nMan i'm tired I think i'm hearing things." });

        ObjectiveManager.instance.ShowObjective("Grab your clothes from the laundry.");
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    // Raycast-based look check
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

    IEnumerator PlayKnockingSequence(float volume)
    {
        if (scareSound != null && knockClip != null)
        {
            float originalVolume = scareSound.volume;
            float originalPitch = scareSound.pitch;

            scareSound.volume = volume;

            if (volume < 0.7f)
            {
                for (int i = 0; i < 2; i++) 
                {
                    scareSound.PlayOneShot(knockClip);
                    yield return new WaitForSeconds(Random.Range(0.4f, 0.7f));
                }
            }
            else
            {
                for (int i = 0; i < 3; i++) 
                {
                    scareSound.pitch = Random.Range(0.8f, 1.0f);
                    scareSound.PlayOneShot(knockClip);
                    yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));
                }
                scareSound.pitch = originalPitch;
            }

            scareSound.volume = originalVolume;
        }
    }
}

