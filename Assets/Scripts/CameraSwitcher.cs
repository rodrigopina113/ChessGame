using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{

    public static CameraSwitcher Instance;

    [Header("Referências às Câmaras")]
    public Camera whiteCamera;
    public Camera blackCamera;

    private void Awake()
    {

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


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
