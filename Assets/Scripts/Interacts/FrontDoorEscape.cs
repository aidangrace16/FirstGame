using UnityEngine;
using TMPro;

public class FrontDoorEscape : MonoBehaviour
{
    public MonsterNavChase mainMonster;
    public UnlockRoommateDoor roommateDoor;
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string promptMessage = "Press E to escape";

    private bool isPlayerInRange = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && mainMonster.mainMonsterChasing && roommateDoor.bedroomDoorOpened)
        {
            isPlayerInRange = true;
            promptText.text = promptMessage;
            promptUI.SetActive(true);
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

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // fire your second main ending (ID = 1)
            EndingManager.instance.ShowEnding("You made it out alive, what the hell was that?", 1);
            foreach (var m in FindObjectsOfType<MonsterNavChase>())
                m.gameObject.SetActive(false);

            // disable the Player object's PlayerMovement script
            var playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
        }
    }
}
