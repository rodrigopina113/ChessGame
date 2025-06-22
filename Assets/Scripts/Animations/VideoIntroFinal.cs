using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroSkip : MonoBehaviour
{
    [Tooltip("Arraste aí seu VideoPlayer (se tiver) ou deixe vazio se não usar vídeo)")]
    public VideoPlayer videoPlayer;

    [Tooltip("Nome exato da cena para carregar quando acabar ou quando o jogador clicar")]
    public string nextSceneName = "MenuScene";

    [Tooltip("Tempo em segundos antes de habilitar o skip por clique/tecla")]
    public float skipCooldown = 3f;

    bool hasSkipped = false;
    float timer = 0f;

    void Start()
    {
        timer = 0f;
        // Se você estiver usando um VideoPlayer, garante transição automática no fim do vídeo
        if (videoPlayer != null)
            videoPlayer.loopPointReached += _ => SkipToNextScene();
    }

    void Update()
    {
        if (hasSkipped) 
            return;

        // Atualiza o contador
        timer += Time.deltaTime;

        // Se ainda não passou do cooldown, sai
        if (timer < skipCooldown)
            return;

        // Mouse / botão principal do mouse
        if (Input.GetMouseButtonDown(0))
            SkipToNextScene();

        // Toque na tela (mobile)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            SkipToNextScene();

        // Qualquer tecla (opcional)
        if (Input.anyKeyDown)
            SkipToNextScene();
    }

    void SkipToNextScene()
    {
        hasSkipped = true;  // evita chamadas múltiplas
        SceneManager.LoadScene(nextSceneName);
    }
}


