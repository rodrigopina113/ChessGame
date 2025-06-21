using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError($"SceneChanger: no sceneName provided on GameObject '{gameObject.name}'");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }
}
