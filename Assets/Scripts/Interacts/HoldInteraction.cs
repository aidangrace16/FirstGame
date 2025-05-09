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

    private bool isPlayerInRange = false;
    private float holdTimer = 0f;
    private bool taskDone = false;

    // spook
    public AudioSource scareSound;
    public AudioClip knockClip;

    // levels during dishes
    private bool quietKnockPlayed = false;
    private bool midKnockTriggered = false;

    void Update()
    {
        if (taskDone || !isPlayerInRange || !ChoreManager.instance.noteHasBeenRead)
            return;


        if (Input.GetKey(KeyCode.E))
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

        // // Optional more Knocks
        // if (holdTimer == holdTime / 2f)
        // {

        //     if (scareSound != null && knockClip != null)
        //     {
        //         scareSound.volume = 0.3f;
        //         StartCoroutine(PlayKnockingSequence(0.5f));
        //     }
        // }

        // 👊 stronger multiple knocks at halfway
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

    void CompleteTask()
    {
        taskDone = true;
        ChoreManager.instance.dishesDone = true;
        chargePanel.SetActive(false);
        DialogueTyper typer = FindObjectOfType<DialogueTyper>();
        typer.PlayDialogue(new string[] { "Dishes done.\n\nMan i'm tired I think i'm hearing things." });

        // NEXT OBJECTIVE
        ObjectiveManager.instance.ShowObjective("Grab your clothes from the laundry.");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (ChoreManager.instance.noteHasBeenRead && !ChoreManager.instance.dishesDone)
            {
                promptText.text = interactionPrompt;
                promptText.gameObject.SetActive(true);
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // holdTimer = 0f;
            // chargeBarFill.fillAmount = 0f;
            chargePanel.SetActive(false);
            promptText.gameObject.SetActive(false);
        }
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
                // softer, early quiet knocks
                for (int i = 0; i < 2; i++) 
                {
                    scareSound.PlayOneShot(knockClip);
                    yield return new WaitForSeconds(Random.Range(0.4f, 0.7f));
                }
            }
            else
            {
                // louder, later knocks
                for (int i = 0; i < 3; i++) 
                {
                    scareSound.pitch = Random.Range(0.8f, 1.0f); // slight pitch variation
                    scareSound.PlayOneShot(knockClip);
                    yield return new WaitForSeconds(Random.Range(0.2f, 0.4f));
                }
                scareSound.pitch = originalPitch; // reset pitch after
            }

            scareSound.volume = originalVolume; // reset volume after
        }
    }


}
