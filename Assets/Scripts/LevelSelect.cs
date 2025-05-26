using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class PlanetButton : MonoBehaviour
{
    public string sceneName;            // Nome da cena a carregar
    public float hoverScale = 1.2f;     
    public float rotationSpeed = 50f;   

    private Vector3 originalScale;
    private bool isHovered = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isHovered)
        {
            // Rotação contínua
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            // Suaviza escala para hoverScale
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale * hoverScale, Time.deltaTime * 8f);
        }
        else
        {
            // Volta à escala original
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 8f);
        }
    }

    void OnMouseEnter()  { isHovered = true;  }
    void OnMouseExit()   { isHovered = false; }

    void OnMouseDown()
    {
        // Carrega a cena configurada
        SceneManager.LoadScene(sceneName);
    }
}

