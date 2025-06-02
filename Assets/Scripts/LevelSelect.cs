using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class PlanetButton : MonoBehaviour
{
    [Header("Configurações de Cena")]
    [Tooltip("Nome da cena que será carregada quando este planeta for clicado")]
    public string sceneName;

    [Tooltip("Chave no PlayerPrefs para verificar se o nível anterior foi concluído. " +
             "Exemplo: se este for Nível 2, coloque 'Level1Completed' para ver se o Level1 está feito.")]
    public string previousLevelKey;

    [Header("Efeito de Hover")]
    [Tooltip("Quanto o planeta escala ao passar o mouse (por exemplo, 1.2 = 20% maior)")]
    public float hoverScale = 1.2f;

    [Tooltip("Velocidade de rotação enquanto estiver em hover")]
    public float rotationSpeed = 50f;

    [Header("Configuração do Glow")]
    [Tooltip("Valor mínimo do alfa (transparência) do glow (0 a 1). Ex: 0.1 para começar quase invisível.")]
    [Range(0f, 1f)]
    public float glowMinAlpha = 0.1f;

    [Tooltip("Valor máximo do alfa (transparência) do glow (0 a 1). Ex: 1 para chegar a 100% de opacidade.")]
    [Range(0f, 1f)]
    public float glowMaxAlpha = 1f;

    [Tooltip("Velocidade do pulso do glow. Ex: 5 para oscilar mais rápido.")]
    public float glowPulseSpeed = 5f;

    [Header("Referências de Objetos Filho")]
    [Tooltip("GameObject que representa o brilho/glow atrás do planeta. Deve ser um filho, posicionado atrás.")]
    public GameObject glowEffect;

    [Tooltip("GameObject do ícone de cadeado que aparece quando o nível estiver bloqueado")]
    public GameObject lockIcon;

    // ---- variáveis internas ----
    private Vector3 originalScale;
    private bool isHovered = false;
    private bool isLocked = false;

    // Componente Renderer do glow, para manipulá-lo dinamicamente
    private Renderer glowRenderer;
    // Cor original do material do glow (sem modificação de alfa)
    private Color glowBaseColor;

    void Start()
    {
        // Guarda a escala original para animar o Lerp depois
        originalScale = transform.localScale;

        // Verifica se o nível anterior (previousLevelKey) foi concluído
        // Se não existir ou for 0 -> continua bloqueado
        if (!string.IsNullOrEmpty(previousLevelKey))
        {
            int prevDone = PlayerPrefs.GetInt(previousLevelKey, 0);
            isLocked = (prevDone != 1);
        }
        else
        {
            // Se não foi definida chave, assume como desbloqueado
            isLocked = false;
        }

        // Atualiza visibilidade do cadeado
        if (lockIcon != null)
            lockIcon.SetActive(isLocked);

        // Prepara o glowEffect (desativa e captura o Renderer se existir)
        if (glowEffect != null)
        {
            glowEffect.SetActive(false);
            glowRenderer = glowEffect.GetComponent<Renderer>();

            if (glowRenderer != null)
            {
                // Cria uma instância própria do material para não mexer no sharedMaterial
                glowRenderer.material = new Material(glowRenderer.material);
                glowBaseColor = glowRenderer.material.color;
                
                // Começa com o alfa no valor mínimo
                Color c = glowBaseColor;
                c.a = glowMinAlpha;
                glowRenderer.material.color = c;
            }
        }
    }

    void Update()
    {
        // Controle de escala e rotação apenas enquanto hover e não estiver bloqueado
        if (isHovered && !isLocked)
        {
            // Rotaciona ao redor do eixo Y em world space
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            // Aumenta suavemente até hoverScale
            transform.localScale = Vector3.Lerp(transform.localScale,
                                                originalScale * hoverScale,
                                                Time.deltaTime * 8f);

            // Se tivermos Renderer do glow, anima o alfa para pulsar intensamente
            if (glowRenderer != null)
            {
                // PingPong varia de 0 a 1 a cada ciclo, com velocidade mais alta
                float t = Mathf.PingPong(Time.time * glowPulseSpeed, 1f);
                float newAlpha = Mathf.Lerp(glowMinAlpha, glowMaxAlpha, t);

                Color c = glowBaseColor;
                c.a = newAlpha;
                glowRenderer.material.color = c;
            }
        }
        else
        {
            // Retorna à escala original
            transform.localScale = Vector3.Lerp(transform.localScale,
                                                originalScale,
                                                Time.deltaTime * 8f);
            // Se não estiver em hover, assegura que o alfa do glow volte ao mínimo
            if (glowRenderer != null && glowEffect.activeSelf)
            {
                Color c = glowBaseColor;
                c.a = glowMinAlpha;
                glowRenderer.material.color = c;
            }
        }
    }

    void OnMouseEnter()
    {
        // Ao passar o mouse, só entra em estado de hover se não estiver bloqueado
        if (!isLocked)
        {
            isHovered = true;

            // Ativa o efeito de glow
            if (glowEffect != null)
                glowEffect.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        // Sai do hover sempre; desativa glow
        isHovered = false;

        if (glowEffect != null)
            glowEffect.SetActive(false);
    }

    void OnMouseDown()
    {
        // Se não estiver bloqueado, carrega a cena. Senão, não faz nada (ou exibe uma mensagem)
        if (!isLocked)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("Nível bloqueado! Termine o anterior primeiro.");
        }
    }

    /// <summary>
    /// Chame externamente quando o jogador concluir este nível:
    /// PlayerPrefs.SetInt("LevelXCompleted", 1);
    /// PlayerPrefs.Save();
    /// </summary>
    public void UnlockNextLevel()
    {
        // Se quiser fazer algo extra ao liberar o próximo, use aqui.
    }
}




