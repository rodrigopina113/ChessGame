using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider volumeSlider;
    public TMP_Dropdown qualityDropdown;
    public GameObject settingsPanel;
    
    [Header("Erase Progress")]
    public Button eraseProgressButton;    // assign no Inspector

private void Start()
{
    // Carrega valor salvo
    float saved = PlayerPrefs.GetFloat("volume", 1f);

    // 1) Associa callback antes de definir o valor
    volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

    // 2) Define o valor (vai mover o handle e chamar OnVolumeChanged)
    volumeSlider.value = saved;

    // 3) Garante que o áudio já fique com o nível correto
    ApplyAudio(saved);

    // Qualidade e erase, se ainda não tiveres feito
    qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
    eraseProgressButton.onClick.AddListener(OnEraseProgressClicked);

    // Carregar valor de qualidade
    qualityDropdown.value = PlayerPrefs.GetInt("quality", QualitySettings.GetQualityLevel());
    ApplyQuality(qualityDropdown.value);
}



    #region Callbacks

    public void OnVolumeChanged(float value)
    {
        ApplyAudio(value);
    }

    public void OnEraseProgressClicked()
    {
        // Opcional: aqui você pode exibir um diálogo de confirmação
        LevelProgressManager.Instance.ResetProgress();
        Debug.Log("Progresso zerado!");
    }

    public void OnQualityChanged(int index)
    {
        ApplyQuality(index);
    }



    #endregion

    public void ApplySettings()
    {
        // Salva no PlayerPrefs
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
        PlayerPrefs.SetInt("quality", qualityDropdown.value);
        PlayerPrefs.Save();
        ClosePanel();
    }

    public void ClosePanel()
    {
        settingsPanel.SetActive(false);
    }

    private void ApplyAudio(float volume)
    {
        // Exemplo usando AudioListener
        AudioListener.volume = volume;
    }

    private void ApplyQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }




}

