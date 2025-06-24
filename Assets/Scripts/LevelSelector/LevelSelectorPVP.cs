using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelDataPVP
{
    public string sceneName;
    public string titulo;
    public string descricao;
    public GameObject model3D;
}

public class LevelSelectorPVP : MonoBehaviour
{
    [Header("Botões de Navegação")]
    public Button leftArrowButton;
    public Button rightArrowButton;
    public Button startButton;

    [Header("Textos UI")]
    public TMP_Text tituloText;
    public TMP_Text descricaoText;

    [Header("Dados dos Níveis")]
    public LevelDataPVP[] niveis;

    private int nivelAtual = 0;

    void Start()
    {
        leftArrowButton.onClick.AddListener(() => TrocarNivel(-1));
        rightArrowButton.onClick.AddListener(() => TrocarNivel(1));
        startButton.onClick.AddListener(IniciarNivelAtual);
        AtualizarUI();
    }

    void TrocarNivel(int direcao)
    {

        niveis[nivelAtual].model3D.SetActive(false);

        nivelAtual += direcao;
        if (nivelAtual < 0) nivelAtual = niveis.Length - 1;
        if (nivelAtual >= niveis.Length) nivelAtual = 0;

        AtualizarUI();
    }

    void AtualizarUI()
    {
        tituloText.text = niveis[nivelAtual].titulo;
        descricaoText.text = niveis[nivelAtual].descricao;


        for (int i = 0; i < niveis.Length; i++)
            niveis[i].model3D.SetActive(i == nivelAtual);
    }

    void IniciarNivelAtual()
    {
        SceneManager.LoadScene(niveis[nivelAtual].sceneName);
    }
}