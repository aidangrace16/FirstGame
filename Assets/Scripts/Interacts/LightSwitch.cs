using UnityEngine;
using TMPro;

public class LightSwitch : MonoBehaviour
{
    public GameObject lightToToggle;
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string promptMessage = "Press E to turn on the light";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip switchClip;

    private bool isPlayerInRange = false;
    private bool lightOn = false;

    void Update()
    {
        if (isPlayerInRange && !lightOn && Input.GetKeyDown(KeyCode.E))
        {
            lightOn = true;

        if (lightToToggle != null)
        {
            lightToToggle.SetActive(true);
            ChoreManager.instance.LightsTurnedOn++;
            ChoreManager.instance.CheckLights();
        }


            if (audioSource != null && switchClip != null)
                audioSource.PlayOneShot(switchClip);

            promptUI.SetActive(false);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !lightOn)
        {
            // only show prompt if ambient light is off (or very dark)
            if (RenderSettings.ambientLight.maxColorComponent <= 0.05f)
            {
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
