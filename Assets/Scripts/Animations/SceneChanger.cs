// SceneChanger.cs
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controla a transição entre níveis incluindo a reprodução
/// da cutscene numa cena dedicada ("CutScene").
/// </summary>
public class SceneChanger : MonoBehaviour
{
    [Tooltip("Filename da cutscene em StreamingAssets/WebGLVideos, ex: 'level1.webm'")]
    public string nextCutsceneFileName;
    
    [Tooltip("Nome da cena do próximo nível, ex: 'Level2-Stonedge'")]
    public string nextLevelSceneName;

    /// <summary>
    /// Prepara os dados da cutscene e carrega a cena "CutScene".
    /// Esta cena deverá conter um CutsceneController que
    /// irá reproduzir o vídeo e depois chamar SceneManager.LoadScene(nextLevelSceneName).
    /// </summary>
    public void PlayNextLevelCutscene()
    {
        // Guarda no loader estático
        NextLevelLoader.cutsceneFileName = nextCutsceneFileName;
        NextLevelLoader.sceneName        = nextLevelSceneName;

        // Carrega a cena dedicada a reprodução de cutscenes
        SceneManager.LoadScene("CutScene");
    }

    /// <summary>
    /// Carrega diretamente uma cena (sem cutscene).
    /// Útil para botões de menu que não tenham cutscene.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"SceneChanger: sceneName não foi definido no GameObject '{gameObject.name}'");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }
}
