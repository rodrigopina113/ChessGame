using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    // Singleton para acesso global
    public static CameraSwitcher Instance;

    [Header("Referências às Câmaras")]
    public Camera whiteCamera;
    public Camera blackCamera;

    private void Awake()
    {
        // Singleton para garantir um único instance do CameraSwitcher
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Ativa a câmara correta com base no turno atual.
    /// </summary>
    public void SwitchCamera(bool isWhiteTurn)
    {
        if (whiteCamera != null && blackCamera != null)
        {
            whiteCamera.enabled = isWhiteTurn;
            blackCamera.enabled = !isWhiteTurn;
        }
        else
        {
            Debug.LogWarning("Câmaras não atribuídas no CameraSwitcher!");
        }
    }
}
