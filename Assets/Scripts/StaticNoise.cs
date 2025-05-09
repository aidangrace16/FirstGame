using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RawImage))]
public class StaticNoise : MonoBehaviour
{
    public int width = 512;
    public int height = 512;
    public Vector2 intervalRange = new Vector2(0.03f, 0.15f);
    public float flickerDuration = 0.05f;
    public float flashAlpha = 0.6f;
    public float fadeOutDuration = 0.2f;
    public float maxRotation = 5f;

    private RawImage rawImage;
    private Texture2D noiseTex;
    private Color32[] pixels;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();
        noiseTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        noiseTex.wrapMode = TextureWrapMode.Repeat;
        noiseTex.filterMode = FilterMode.Point;
        rawImage.texture = noiseTex;
        rawImage.color = new Color(1, 1, 1, 0);
        pixels = new Color32[width * height];
    }

    void OnEnable()
    {
        StartCoroutine(NoiseLoop());
    }

    IEnumerator NoiseLoop()
    {
        while (true)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                byte v = (byte)Random.Range(0, 256);
                pixels[i] = new Color32(v, v, v, v);
            }
            noiseTex.SetPixels32(pixels);
            noiseTex.Apply();

            rawImage.uvRect = new Rect(Random.value, Random.value, 1, 1);
            transform.localEulerAngles = new Vector3(0, 0, Random.Range(-maxRotation, maxRotation));

            rawImage.canvasRenderer.SetAlpha(flashAlpha);
            yield return new WaitForSeconds(Random.Range(intervalRange.x, intervalRange.y));

            rawImage.CrossFadeAlpha(0, fadeOutDuration, false);
            yield return new WaitForSeconds(Random.Range(intervalRange.x, intervalRange.y));
        }
    }
}
