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

    public Transform doorVisual; // Assign the moving part of the door here
    public Transform openStateTransform; // Assign a reference transform with the open position/rotation

    private bool isPlayerInRange = false;
    private bool isPlayerInRangeFIRST = false;
    private bool doorUnlocked = false;
    private bool isOpen = false;


    [Header("Secret Ending")]
    public GameObject specialMonster;
    private int secretThreshold = 30;      // how many “…” presses
    private int secretCount = 0;
    private bool secretTriggered = false;

    [HideInInspector]
    public bool bedroomDoorOpened = false;

    [Header("Interaction")]
    public float interactDistance = 3f; // How far the player can interact
    public float sphereCastRadius = 0.4f; // How wide the interaction "cone" is


    [Header("Scare to enable")]
    public GameObject scareToEnable;
    public GameObject scareToDisable;

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && IsPlayerLookingAtDoor())
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
                    scareToEnable.SetActive(true);
                    scareToDisable.SetActive(false);
                    ChoreManager.instance.canPickUpKey = true;
                }
            }
            else
            {
                ToggleDoor(); // acts like DoorInteract now
            }
        }
        else if (isPlayerInRangeFIRST && Input.GetKeyDown(KeyCode.E) && IsPlayerLookingAtDoor())
        {
            if (!secretTriggered)
            {
                secretCount++;

                DialogueTyper typer = FindObjectOfType<DialogueTyper>();
                
                if (secretCount == 1) {
                    typer.PlayDialogue(new string[] { "I shouldn't go in there..." });
                } else if (secretCount == 14) {
                    int dotCount = Random.Range(10, 20);
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
                    typer.PlayDialogue(new string[] { "Aw hell naw the door gone" });
                    // spawn/enable your special monster
                    if (specialMonster != null)
                        specialMonster.SetActive(true);
                }
            }
        }

        // Handle promptUI visibility based on look direction
        if (isPlayerInRange && promptUI != null)
        {
            if (IsPlayerLookingAtDoor())
            {
                if (!promptUI.activeSelf)
                    promptUI.SetActive(true);
            }
            else
            {
                if (promptUI.activeSelf)
                    promptUI.SetActive(false);
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
            bedroomDoorOpened = true;
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

    // Returns true if the player is looking at this door (with a forgiving sphere cast)
    bool IsPlayerLookingAtDoor()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hit;
        if (Physics.SphereCast(ray, sphereCastRadius, out hit, interactDistance))
        {
            if (hit.collider == doorCollider)
                return true;
        }
        return false;
    }

    void Start()
    {
        if (PlayerPrefs.GetInt("AllEndingsFound", 0) == 1)
        {
            isOpen = true;
            doorUnlocked = true;
            if (animator != null)
            {
                animator.SetBool("isOpen", true);
                animator.Update(0f); // Snap to open state
                animator.enabled = false; // Prevent animation from playing
            }
            if (doorVisual != null)
            {
                doorVisual.localRotation = Quaternion.Euler(0f, -120f, 0f);
            }
            if (doorCollider != null)
                doorCollider.enabled = false;
            // Optionally hide prompt UI
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }
}
