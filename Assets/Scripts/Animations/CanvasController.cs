using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject canvasSecundario;

    public void MostrarCanvasSecundario()
    {
        if (canvasSecundario != null)
        {
            canvasSecundario.SetActive(true);
        }
    }
}
