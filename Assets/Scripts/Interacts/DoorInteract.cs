using UnityEngine;
using TMPro;

public class DoorInteract : MonoBehaviour
{
    public Animator animator;
    public GameObject promptUI;
    public TextMeshProUGUI promptText;
    public string promptMessage;

    public AudioClip openSound;
    public AudioClip closeSound;
    private AudioSource audioSource;

    public Collider doorCollider; // drag your door's collider here in inspector

    private bool isPlayerInRange = false;
    private bool isOpen = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            isOpen = !isOpen;
            animator.SetBool("isOpen", isOpen);

            // Stop current audio
            audioSource.Stop();
            if (isOpen && openSound != null)
            {
                audioSource.clip = openSound;
                audioSource.Play();
                doorCollider.enabled = false;
            }
            else if (!isOpen && closeSound != null)
            {
                audioSource.clip = closeSound;
                audioSource.Play();
                doorCollider.enabled = true;
            }
        }
    }

    public void TriggerDoor()
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
        if (other.CompareTag("Player"))
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


    
}
