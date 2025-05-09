using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager instance;

    public TextMeshProUGUI objectiveText;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        objectiveText.text = "";
    }

    public void ShowObjective(string text)
    {
        objectiveText.text = "Objective: " + text;
    }

    public void ClearObjective()
    {
        objectiveText.text = "";
    }
}
