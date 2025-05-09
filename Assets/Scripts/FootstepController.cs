using UnityEngine;

public class FootstepController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] walkSounds;
    public AudioClip[] runSounds;

    public PlayerMovement playerMovement; // Drag your PlayerMovement script here!

    private float lastBobValue = 0f;
    private bool footstepPlayed = false;

    void Update()
    {
        bool isMoving = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;

        if (isMoving)
        {
            float currentBobValue = Mathf.Sin(playerMovement.bobTimer);

            // When bobbing crosses downward (like stepping down)
            if (currentBobValue < 0 && lastBobValue >= 0 && !footstepPlayed)
            {
                PlayFootstep();
                footstepPlayed = true;
            }

            // Reset when the bobbing comes back up
            if (currentBobValue > 0 && lastBobValue <= 0)
            {
                footstepPlayed = false;
            }

            lastBobValue = currentBobValue;
        }
    }

    void PlayFootstep()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        AudioClip[] soundArray = isRunning ? runSounds : walkSounds;

        if (soundArray.Length > 0)
        {
            audioSource.PlayOneShot(soundArray[Random.Range(0, soundArray.Length)]);
        }
    }
}
