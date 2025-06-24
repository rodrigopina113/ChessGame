using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.IO;

public class SceneChanger : MonoBehaviour
{
    [Tooltip("Filename of your cutscene WebM, e.g. 'level1.webm'")]
    public string nextCutsceneFileName;
    [Tooltip("Defina no Inspector o nome da cena do próximo nível")]
    public string nextLevelSceneName;
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"SceneChanger: no sceneName provided on GameObject '{gameObject.name}'");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

        public void PlayNextLevelCutscene()
    {
        // Build the URL to StreamingAssets/WebGLVideos/<your file>
        string basePath = Application.streamingAssetsPath;
        string url      = Path.Combine(basePath, "WebGLVideos", nextCutsceneFileName);

        // Find and configure the VideoPlayer in the CutScene scene
        var vp = FindFirstObjectByType<VideoPlayer>();
        vp.source = VideoSource.Url;
        vp.url    = url;
        vp.Play();

        // After it finishes, load the next level
        vp.loopPointReached += _ => 
            SceneManager.LoadScene(nextLevelSceneName);
    }
}
