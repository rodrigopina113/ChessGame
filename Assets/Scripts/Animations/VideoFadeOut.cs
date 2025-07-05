using UnityEngine;
using UnityEngine.Video;
using TMPro;
using System.Collections;

[RequireComponent(typeof(VideoPlayer))]
public class VideoLoadingManager : MonoBehaviour
{
    [Header("Referências")]
    public GameObject loadingScreenGO;
    public TextMeshProUGUI loadingText;

    [Header("Configuração")]
    public float dotInterval = 0.5f;
    public float loadingDuration = 1.5f;  // Duração fixa em segundos

    private VideoPlayer vp;
    private float timer = 0f;
    private int dotCount = 1;

    void Awake()
    {
        vp = GetComponent<VideoPlayer>();
    }

    void Start()
    {
        vp.loopPointReached += OnVideoEnd;
        StartCoroutine(ShowLoadingAndPlayVideo());
    }

    IEnumerator ShowLoadingAndPlayVideo()
    {
        // 1) Mostra o loading
        loadingScreenGO.SetActive(true);
        timer = 0f;
        dotCount = 1;
        loadingText.text = "Loading.";

        // 2) Anima os pontos durante 1.5s
        float elapsed = 0f;
        while (elapsed < loadingDuration)
        {
            elapsed += Time.deltaTime;
            timer += Time.deltaTime;

            if (timer >= dotInterval)
            {
                timer = 0f;
                dotCount = (dotCount % 3) + 1;
                loadingText.text = "Loading" + new string('.', dotCount);
            }

            yield return null;
        }

        // 3) Esconde o loading e começa o vídeo
        loadingScreenGO.SetActive(false);
        vp.Play();
    }

    private void OnVideoEnd(VideoPlayer source)
    {
        // Aqui podes voltar a mostrar loading, etc.
    }
}
