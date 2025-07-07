// CutsceneController.cs
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    [Header("Video Player Settings")]
    public VideoPlayer videoPlayer;
    public Renderer    quadRenderer;
    public string      materialProperty = "_BaseMap";
    public float       skipCooldown     = 1f;

    bool hasStarted = false;
    bool canSkip => Time.timeSinceLevelLoad >= skipCooldown;

    void Start()
    {
        if (string.IsNullOrEmpty(NextLevelLoader.cutsceneFileName))
        {
            Debug.LogError("[CutsceneController] cutsceneFileName está vazio!");
            return;
        }

        // Configura para URL
        videoPlayer.source = VideoSource.Url;
        // Monta o caminho para StreamingAssets/WebGLVideos
        string url = Application.streamingAssetsPath
                   + "/WebGLVideos/"
                   + NextLevelLoader.cutsceneFileName;
        videoPlayer.url = url;

        // Renderiza sobre o material do quad
        videoPlayer.renderMode             = VideoRenderMode.MaterialOverride;
        videoPlayer.targetMaterialRenderer = quadRenderer;
        videoPlayer.targetMaterialProperty = materialProperty;

        videoPlayer.loopPointReached += _ => LoadLevel();
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += vp => {
            vp.Play();
            hasStarted = true;
        };
    }

    void Update()
    {
        if (hasStarted && canSkip &&
            (Input.GetMouseButtonDown(0) || Input.anyKeyDown))
        {
            LoadLevel();
        }
    }

    void LoadLevel()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        // Reset camera properties before scene transition
        if (Camera.main != null)
        {
            Camera.main.clearFlags = CameraClearFlags.Skybox;
            Camera.main.backgroundColor = Color.black;
            Debug.Log("[CutsceneController] Reset camera for WebGL before scene transition");
        }
        #endif
        
        SceneManager.LoadScene(NextLevelLoader.sceneName);
    }
}
