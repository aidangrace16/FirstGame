using UnityEngine;
using UnityEngine.Video;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/GlitchEffect")]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(VideoPlayer))]
public class VHSPostProcessEffect : MonoBehaviour
{
    public Shader shader;
    public VideoClip VHSClip;

    private Material _material;
    private VideoPlayer _player;

    private float _yScanline;
    private float _xScanline;
    private float timerY;

    void OnEnable()
    {
        if (shader != null)
            _material = new Material(shader);
    }

    void Start()
    {
        _player = GetComponent<VideoPlayer>();
        _player.isLooping       = true;
        _player.renderMode      = VideoRenderMode.APIOnly;
        _player.audioOutputMode = VideoAudioOutputMode.None;
        _player.clip            = VHSClip;
        _player.Play();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (_material == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        _material.SetTexture("_VHSTex", _player.texture);

        // example scanline logic (adjust to taste)
        timerY += Time.deltaTime * 0.3f;
        _xScanline -= Time.deltaTime * 0.1f;

        if (timerY >= 0.3f)
        {
            _yScanline = Random.Range(0f, 3f);
            timerY    = Random.Range(0f, 2f);
        }

        if (_xScanline <= 0f || Random.value < 0.05f)
            _xScanline = Random.Range(0.05f, 0.1f);

        _material.SetFloat("_yScanline", _yScanline);
        _material.SetFloat("_xScanline", _xScanline);

        Graphics.Blit(source, destination, _material);
    }

    void OnDisable()
    {
        if (_material)
        {
            DestroyImmediate(_material);
            _material = null;
        }
    }
}
