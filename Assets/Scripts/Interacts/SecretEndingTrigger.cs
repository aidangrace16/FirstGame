using UnityEngine;
using TMPro;

public class SecretEndingTrigger : MonoBehaviour
{
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string promptMessage = "Press E to sleep";
    public string endingText;
    public int endingID = 2;
    public float interactDistance = 3f;
    public float sphereCastRadius = 0.4f;

    private bool isPlayerInRange = false;
    private bool hasInteracted = false;

    void Update()
    {
        if (hasInteracted)
            return;

        if (!AllLightsOffAndAmbientDark())
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
                if (!string.IsNullOrEmpty(endingText) && EndingManager.instance != null)
                {
                    EndingManager.instance.ShowEnding(endingText, endingID);
                    hasInteracted = true;
                    if (promptUI != null && promptUI.activeSelf)
                        promptUI.SetActive(false);
                }
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

    bool AllLightsOffAndAmbientDark()
    {
        var switches = FindObjectsOfType<LightSwitch>();
        foreach (var sw in switches)
        {
            var lightOnField = sw.GetType().GetField("lightOn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (lightOnField != null && (bool)lightOnField.GetValue(sw))
                return false;
        }
        if (RenderSettings.ambientLight.maxColorComponent >= 0.05f)
            return false;
        return true;
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
