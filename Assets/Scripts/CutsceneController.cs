using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
        public Renderer    quadRenderer;
    public string      materialProperty = "_BaseMap";
    public float skipCooldown = 1f;
    bool hasStarted = false;
    bool canSkip => Time.timeSinceLevelLoad >= skipCooldown;

    void Start()
    {
        if (NextLevelLoader.cutsceneClip == null)
        {
            Debug.LogError("[Cutscene] clip é NULL!");
            return;
        }

        videoPlayer.source                 = VideoSource.VideoClip;
        videoPlayer.clip                   = NextLevelLoader.cutsceneClip;
        videoPlayer.renderMode             = VideoRenderMode.MaterialOverride;
        videoPlayer.targetMaterialRenderer = quadRenderer;
        videoPlayer.targetMaterialProperty = materialProperty;

        videoPlayer.loopPointReached += _ => LoadLevel();
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted  += vp => vp.Play();
        hasStarted = true;
    }

    void Update()
    {
        if (hasStarted && canSkip &&
            (Input.GetMouseButtonDown(0) || Input.anyKeyDown))
            LoadLevel();
    }

    void LoadLevel()
    {
        SceneManager.LoadScene(NextLevelLoader.sceneName);
    }
}

