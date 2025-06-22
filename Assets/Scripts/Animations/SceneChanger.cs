using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SceneChanger : MonoBehaviour
{
    [Tooltip("Defina no Inspector o clipe da cutscene do próximo nível")]
    public VideoClip nextCutsceneClip;
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
        // 1) Passa dados para o loader estático
        NextLevelLoader.sceneName    = nextLevelSceneName;
        NextLevelLoader.cutsceneClip = nextCutsceneClip;
        // 2) Carrega a cena de cutscene
        SceneManager.LoadScene("CutScene");
    }
}
