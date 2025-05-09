using System.Collections;
using UnityEngine;

public class ToggleLightsTrigger : MonoBehaviour
{
    [Header("Toggle Options")]
    public bool turnOffLights = true;
    public bool turnOnLights = false;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip powerDownClip;

    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player") && ChoreManager.instance.laundryPlaced)
        {
            triggered = true;

            if (turnOffLights)
                RenderSettings.ambientLight = new Color(0.04f, 0.04f, 0.04f);
            else if (turnOnLights)
                RenderSettings.ambientLight = Color.white;

            if (audioSource != null && powerDownClip != null)
                audioSource.PlayOneShot(powerDownClip);

            StartCoroutine(HandlePostLightsOff());
        }
    }

    IEnumerator HandlePostLightsOff()
    {
        yield return new WaitForSeconds(1f);

        DialogueTyper typer = FindObjectOfType<DialogueTyper>();
        typer.PlayDialogue(new string[] { "...what the hell?" });

        ObjectiveManager.instance.ShowObjective("Turn on all the lights in the house.");
    }


}
