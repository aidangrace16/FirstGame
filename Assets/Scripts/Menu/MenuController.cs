using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    // drag your gameplay scene name here in the inspector:
    public string gameSceneName = "GameScene";
    public Transform menuDoorVisual; // Assign the menu door's moving part
    public Animator animator; // Assign the Animator component for the door
    public Vector3 openRotation = new Vector3(0f, -120f, 0f); // Set to your open rotation

    void Start()
    {
        if (PlayerPrefs.GetInt("AllEndingsFound", 0) == 1 && menuDoorVisual != null)
        {

            if (animator != null)
            {
                animator.SetBool("isOpen", true);
                animator.Update(0f); // Snap to open state
                animator.enabled = false; // Prevent animation from playing
            }
            Debug.Log("All endings found, opening menu door.");
            menuDoorVisual.localRotation = Quaternion.Euler(openRotation);
        }
    }

    public void OnPlayPressed()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
