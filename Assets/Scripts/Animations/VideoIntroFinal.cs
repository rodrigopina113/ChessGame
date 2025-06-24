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

        if (videoPlayer != null)
            videoPlayer.loopPointReached += _ => SkipToNextScene();
    }

    void Update()
    {
        if (hasSkipped) 
            return;


        timer += Time.deltaTime;


        if (timer < skipCooldown)
            return;


        if (Input.GetMouseButtonDown(0))
            SkipToNextScene();


        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            SkipToNextScene();


        if (Input.anyKeyDown)
            SkipToNextScene();
    }

    void SkipToNextScene()
    {
        hasSkipped = true;
        SceneManager.LoadScene(nextSceneName);
    }
}


