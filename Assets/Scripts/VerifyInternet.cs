using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;     // se for TextMeshPro
// using UnityEngine.UI; // se for o Text legacy

public class VerifyInternet : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject        panelVerificar;  // o panel que cobre a tela
    [SerializeField] private TextMeshProUGUI   textoVerificar;  // ou Text se for UI.Text

    [Header("Configurações")]
    [SerializeField] private string  cenaAlvo      = "LevelSelect";
    [SerializeField] private string  urlTeste      = "https://www.google.com";
    [SerializeField] private int     timeout       = 5;     // segundos de timeout
    [SerializeField] private float   displayTime   = 2f;   // quanto tempo exibe o erro

    public void OnCheckButtonClicked()
    {
        // 1) ativa o painel e mostra “Verificando…”
        panelVerificar.SetActive(true);
        textoVerificar.text = "Verificando conexão…";

        // 2) faz UMA checagem e sai
        StartCoroutine(CheckAndMaybeProceed());
    }

    private IEnumerator CheckAndMaybeProceed()
    {
        // A) checa conexão física
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            textoVerificar.text = "Sem conexão de rede";
            yield return new WaitForSeconds(displayTime);
            panelVerificar.SetActive(false);
            yield break;
        }

        // B) checa internet de verdade via HEAD request
        using (var www = UnityWebRequest.Head(urlTeste))
        {
            www.timeout = timeout;
            yield return www.SendWebRequest();

            bool falha = www.result == UnityWebRequest.Result.ConnectionError
                      || www.result == UnityWebRequest.Result.ProtocolError
                      || www.responseCode != 200;

            if (!falha)
            {
                // se OK, carrega cena imediatamente
                SceneManager.LoadScene(cenaAlvo);
            }
            else
            {
                // se falhou, mostra o erro e esconde o painel depois de displayTime
                textoVerificar.text = "Sem acesso à Internet";
                yield return new WaitForSeconds(displayTime);
                panelVerificar.SetActive(false);
            }
        }
    }
}
