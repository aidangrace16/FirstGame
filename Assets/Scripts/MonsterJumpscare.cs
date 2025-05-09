using UnityEngine;
using System.Collections;

public class MonsterJumpscare : MonoBehaviour
{
    public CanvasGroup fadePanel;
    public float fadeSpeed = 1f;

    public int endingID = 0;

    string endingText = "";

    public void TriggerFadeToEnding(float delay = 0f)
    {
        StartCoroutine(FadeToBlack(delay));
    }

    IEnumerator FadeToBlack(float delay)
    {
        yield return new WaitForSeconds(delay); // ⬅️ delay BEFORE fade starts

        fadePanel.gameObject.SetActive(true);

        while (fadePanel.alpha < 1f)
        {
            fadePanel.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        if (endingID == 0) {
            endingText = "None of your remains were found.\n\n";

        } else if (endingID == 3) {
            endingText = "Bruh\n\n";

        } else {
            endingText = "You died.\n\n"; // fallback (optional)
        }

        EndingManager.instance.ShowEnding(endingText, endingID);

    }
}
