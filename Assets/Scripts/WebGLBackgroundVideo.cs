using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class WebGLIntroVideo : MonoBehaviour
{
    [Tooltip("Name of your video file in StreamingAssets")]
    public string videoFileName = "intro.mp4";

    private VideoPlayer vp;

    void Awake()
    {
        vp = GetComponent<VideoPlayer>();

        // 1) URL source
        vp.source = VideoSource.Url;
        vp.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);

        // 2) Render to camera background
        vp.renderMode = VideoRenderMode.CameraFarPlane;
        vp.targetCamera = Camera.main;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;

        // 3) Don’t auto-play until we call Play()
        vp.playOnAwake = false;
        vp.isLooping = true;

        // 4) Mute it so browser will autoplay
        #if UNITY_WEBGL
        vp.audioOutputMode = VideoAudioOutputMode.None;
        #endif

        // 5) Wait for first frame before playing
        vp.waitForFirstFrame = true;
        vp.prepareCompleted += OnVideoPrepared;
    }

    void Start()
    {
        // Kicks off the async load
        vp.Prepare();
    }

    void OnVideoPrepared(VideoPlayer source)
    {
        source.Play();
    }
}
