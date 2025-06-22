using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EndingManager : MonoBehaviour
{
    public static EndingManager instance;
    public GameObject endingScreen;
    public TextMeshProUGUI endingText;

    public float returnDelay = 8f; // how long to wait before returning to the main menu

    // define which IDs are main vs secret:
    private int[] mainIDs   = { 0, 1 };
    private int[] secretIDs = { 2, 3 }; // Only two secret endings

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // debug: press R to reset all endings
        if (Input.GetKeyDown(KeyCode.R))
            ResetEndings();
    }

    // FOR DEBUGGING ENDINGS
    public void ResetEndings()
    {
        PlayerPrefs.DeleteKey("AllEndingsFound");

        // delete every Ending_<ID> key
        foreach (var id in mainIDs)
            PlayerPrefs.DeleteKey("Ending_" + id);
        foreach (var id in secretIDs)
            PlayerPrefs.DeleteKey("Ending_" + id);

        PlayerPrefs.Save();
        Debug.Log("Endings reset");
    }

    public void ShowEnding(string text, int endingID)
    {
        // mark this one unlocked
        string key = "Ending_" + endingID;
        if (!PlayerPrefs.HasKey(key))
            PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();

        // count found
        int mainFound = 0, secretFound = 0;
        foreach (var id in mainIDs)
            if (PlayerPrefs.HasKey("Ending_" + id))
                mainFound++;
        foreach (var id in secretIDs)
            if (PlayerPrefs.HasKey("Ending_" + id))
                secretFound++;

        // show screen
        endingScreen.SetActive(true);
        endingText.text = text
            + "\n[" + mainFound   + "/" + mainIDs.Length   + " main endings found]"
            + "\n[" + secretFound + "/" + secretIDs.Length + " secret endings found]";

        // If all endings are found, set PlayerPrefs key
        if (mainFound == mainIDs.Length && secretFound == secretIDs.Length)
        {
            PlayerPrefs.SetInt("AllEndingsFound", 1);
            PlayerPrefs.Save();
            Debug.Log("All endings found!");
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        // schedule return to MainMenu
        Invoke(nameof(LoadMainMenu), returnDelay);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
