using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class WebGLIntroVideo : MonoBehaviour
{
    [Tooltip("Arraste aqui o VideoClip desejado (importado no Unity)")]
    public VideoClip videoClip;

    private VideoPlayer vp;

    void Awake()
    {
        vp = GetComponent<VideoPlayer>();

        // 1) Fonte: VideoClip
        vp.source = VideoSource.VideoClip;

        // 2) Atribuir o clip diretamente
        if (videoClip == null)
        {
            Debug.LogError("VideoClip não atribuído no Inspector!");
            return;
        }
        vp.clip = videoClip;

        // 3) Renderizar no fundo da câmera (opcional)
        vp.renderMode = VideoRenderMode.CameraFarPlane;
        vp.targetCamera = Camera.main;
        if (Camera.main != null)
            Camera.main.clearFlags = CameraClearFlags.SolidColor;

        // 4) Configurações
        vp.playOnAwake = true;    // toca automaticamente ao iniciar
        vp.isLooping = true;

        // 5) Mute para WebGL autoplay
        #if UNITY_WEBGL
        vp.audioOutputMode = VideoAudioOutputMode.None;
        #endif
    }
}
