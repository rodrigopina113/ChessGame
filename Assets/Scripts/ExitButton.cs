using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void ExitGame()
    {
#if UNITY_EDITOR
        // Para parar o jogo no editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Para fechar o jogo no build
        Application.Quit();
#endif
    }
}
