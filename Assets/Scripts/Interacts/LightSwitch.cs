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
    public float interactDistance = 3f;

    void Update()
    {
        if (lightOn)
        {
            // Only turn off promptUI if it's showing this prompt
            if (promptUI.activeSelf && promptText.text == promptMessage)
                promptUI.SetActive(false);
            return;
        }

        bool isLooking = IsPlayerLookingAtObject();
        if (isLooking && RenderSettings.ambientLight.maxColorComponent <= 0.05f)
        {
            if (!promptUI.activeSelf || promptText.text != promptMessage)
            {
                promptText.text = promptMessage;
                promptUI.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.E))
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
                // Only turn off promptUI if it's showing this prompt
                if (promptUI.activeSelf && promptText.text == promptMessage)
                    promptUI.SetActive(false);
            }
        }
        else
        {
            // Only turn off promptUI if it's showing this prompt
            if (promptUI.activeSelf && promptText.text == promptMessage)
                promptUI.SetActive(false);
        }
    }

    bool IsPlayerLookingAtObject()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider == GetComponent<Collider>())
                return true;
        }
        return false;
    }
}
