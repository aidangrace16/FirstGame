using UnityEngine;

public class ChoreManager : MonoBehaviour
{
    public static ChoreManager instance;

    // Read note
    public bool noteHasBeenRead = false;

    // Chore 1: Dishes
    public bool dishesDone = false;

    // Chore 2: Laundry
    public bool holdingLaundry = false;
    public bool laundryPlaced = false;

    // Chore 3: Turn on Lights
    public int totalLightsToTurnOn = 4;
    public int LightsTurnedOn = 0;

    public bool allLightsRestored = false;


    // Chore 4: Get key
    public bool canPickUpKey = false;

    public bool hasLockpick = false;



    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void CheckLights()
    {
        if (LightsTurnedOn >= totalLightsToTurnOn)
        {
            Debug.Log("All lights are back on!");
            // Here you can unlock the roommate room, trigger dialogue, whatever
            ObjectiveManager.instance.ShowObjective("Check your roommate's room.");
            DialogueTyper typer = FindObjectOfType<DialogueTyper>();
            typer.PlayDialogue(new string[] { "All the lights are back on. " });

            // you could also unlock the door here, play sound, etc
            allLightsRestored = true;
        }
    }
    

}
