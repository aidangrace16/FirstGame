using UnityEngine;
using TMPro;

public class UnlockRoommateDoor : MonoBehaviour
{
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string lockedPrompt = "Press E to try door";
    public string unlockPrompt = "Press E to unlock door";

    public AudioSource audioSource;
    public AudioClip unlockSound;
    public AudioClip openSound;
    public AudioClip closeSound;

    public Animator animator;
    public Collider doorCollider;

    public GameObject heldLockpick;
    public GameObject monsterToEnable;

    private bool isPlayerInRange = false;
    private bool isPlayerInRangeFIRST = false;
    private bool doorUnlocked = false;
    private bool isOpen = false;


    [Header("Secret Ending")]
    public GameObject specialMonster;
    private int secretThreshold = 20;      // how many “…” presses
    private int secretCount = 0;
    private bool secretTriggered = false;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!doorUnlocked)
            {
                if (ChoreManager.instance.hasLockpick)
                {
                    UnlockDoor();
                }
                else
                {
                    DialogueTyper typer = FindObjectOfType<DialogueTyper>();
                    typer.PlayDialogue(new string[] { "It's locked... I need to find a way to open it." });
                    ObjectiveManager.instance.ShowObjective("Find something to open the door.");
                    ChoreManager.instance.canPickUpKey = true;
                }
            }
            else
            {
                ToggleDoor(); // acts like DoorInteract now
            }
        }
        else if (isPlayerInRangeFIRST && Input.GetKeyDown(KeyCode.E))
        {
            if (!secretTriggered)
            {
                secretCount++;

                DialogueTyper typer = FindObjectOfType<DialogueTyper>();
                
                if (secretCount == 1) {
                    typer.PlayDialogue(new string[] { "I shouldn't go in there..." });
                } else if (secretCount == 3 || secretCount == 8 || secretCount == 14) {
                    int dotCount = Random.Range(4, 20);
                    string dots  = new string('.', dotCount);
                    typer.PlayDialogue(new string[]{ dots });

                } else if (secretCount < secretThreshold) {
                    typer.PlayDialogue(new string[] { "..." });
                }
                
                if (secretCount >= secretThreshold)
                {
                    secretTriggered = true;
                    // vanish the door
                    gameObject.SetActive(false);
                    typer.PlayDialogue(new string[] { "Aw hell naw the door gone af" });
                    // spawn/enable your special monster
                    if (specialMonster != null)
                        specialMonster.SetActive(true);
                }
            }
        }
    }

    void UnlockDoor()
    {
        doorUnlocked = true;

        if (audioSource != null && unlockSound != null)
            audioSource.PlayOneShot(unlockSound);

        if (promptUI != null)
            promptUI.SetActive(false);

        DialogueTyper typer = FindObjectOfType<DialogueTyper>();
        typer.PlayDialogue(new string[] { "It's unlocked." });

        heldLockpick.SetActive(false);

        if (monsterToEnable != null)
            monsterToEnable.SetActive(true);
    }

    void ToggleDoor()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);

        audioSource.Stop();
        if (isOpen && openSound != null)
        {
            audioSource.clip = openSound;
            audioSource.Play();
        }
        else if (!isOpen && closeSound != null)
        {
            audioSource.clip = closeSound;
            audioSource.Play();
        }

        doorCollider.enabled = false;
        Invoke(nameof(EnableCollider), 1f);
    }

    void EnableCollider()
    {
        doorCollider.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && ChoreManager.instance.allLightsRestored)
        {
            isPlayerInRange = true;
            if (!doorUnlocked && promptUI != null)
            {
                if (ChoreManager.instance.hasLockpick)
                    promptText.text = unlockPrompt;
                else
                    promptText.text = lockedPrompt;

                promptUI.SetActive(true);
            } else if (doorUnlocked && promptUI != null) {
                // promptText.text = "Press E";
                // promptUI.SetActive(true);
            }
        }
        else if (other.CompareTag("Player"))
        {
            isPlayerInRangeFIRST = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            isPlayerInRangeFIRST = false;

            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }
}
