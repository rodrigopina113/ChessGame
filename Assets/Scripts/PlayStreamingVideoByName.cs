using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class PlayStreamingVideoByClip : MonoBehaviour
{
    [Tooltip("Arrasta aqui o VideoClip desejado (importado no Unity)")]
    public VideoClip videoClip;

    private VideoPlayer vp;

    void Awake()
    {
        vp = GetComponent<VideoPlayer>();

        // 1) Alterar fonte para VideoClip
        vp.source = VideoSource.VideoClip;

        // 2) Atribuir o clip diretamente
        vp.clip = videoClip;

        // 3) Configurações opcionais
        vp.playOnAwake = true;
        vp.isLooping = true;
    }
}
