using UnityEngine;
using System.IO;

public class CaptureNoteTexture : MonoBehaviour
{
    public Camera noteCamera; // Assign your UI camera here
    public RenderTexture renderTexture; // Assign your RenderTexture here

    [ContextMenu("Capture Note")]
    public void CaptureNote()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        noteCamera.targetTexture = renderTexture;
        noteCamera.Render();

        Texture2D image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        image.Apply();

        byte[] bytes = image.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/NoteCapture.png", bytes);

        RenderTexture.active = currentRT;
        noteCamera.targetTexture = null;

        Debug.Log("Note captured to " + Application.dataPath + "/NoteCapture.png");
    }
}
