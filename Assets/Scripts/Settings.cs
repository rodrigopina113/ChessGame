using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider volumeSlider;
    public TMP_Dropdown qualityDropdown;
    public TMP_Text qualityLabel;    // opcional, para mostrar “Low” ou “High” na UI
    public GameObject settingsPanel;
    public GameObject menuPanel;

    [Header("Buttons")]
    public Button applyButton;
    public Button cancelButton;
    public Button eraseProgressButton;

    [Header("Confirm Dialog")]
    public GameObject confirmPanel;      // painel de confirmação
    public Button confirmYesButton;      // botão "Sim"
    public Button confirmNoButton;       // botão "Não"

    // Índices internos para presets
    private const int PRESET_LOW = 0;
    private const int PRESET_HIGH = 1;

    private void Start()
    {
        // Carrega preset salvo (0 = Low, 1 = High)
        int savedPreset = PlayerPrefs.GetInt("quality", PRESET_HIGH);

        // Configurar opções do dropdown
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string> { "Low", "High" });

        // Ligação de callbacks
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityPresetChanged);
        eraseProgressButton.onClick.AddListener(ShowConfirmPanel);
        applyButton.onClick.AddListener(OnApplyClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);

        // Configurar o painel de confirmação inicialmente escondido
        confirmPanel.SetActive(false);
        confirmYesButton.onClick.AddListener(OnConfirmYesClicked);
        confirmNoButton.onClick.AddListener(OnConfirmNoClicked);

        // Aplica valores iniciais
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);
        OnVolumeChanged(volumeSlider.value);

        qualityDropdown.value = savedPreset;
        ApplyQualityPreset(savedPreset);
    }

    #region Callbacks

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    private void OnQualityPresetChanged(int presetIndex)
    {
        ApplyQualityPreset(presetIndex);
    }

    private void ShowConfirmPanel()
    {
        // Exibe painel de confirmação antes de apagar progresso
        confirmPanel.SetActive(true);
    }

    private void OnConfirmYesClicked()
    {
        // Usuário confirmou
        LevelProgressManager.Instance.ResetProgress();
        Debug.Log("Progresso zerado!");
        confirmPanel.SetActive(false);
    }

    private void OnConfirmNoClicked()
    {
        // Usuário cancelou
        confirmPanel.SetActive(false);
    }

    private void OnApplyClicked()
    {
        // Salva volume e preset
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
        PlayerPrefs.SetInt("quality", qualityDropdown.value);
        PlayerPrefs.Save();

        ShowMenuPanel();
    }

    private void OnCancelClicked()
    {
        // Sem salvar, volta ao menu
        ShowMenuPanel();
    }

    #endregion

    private void ApplyQualityPreset(int presetIndex)
    {
        int lowest = 0;
        int highest = QualitySettings.names.Length - 1;

        if (presetIndex == PRESET_LOW)
            QualitySettings.SetQualityLevel(lowest);
        else
            QualitySettings.SetQualityLevel(highest);

        // Atualiza label opcional
        if (qualityLabel != null)
            qualityLabel.text = presetIndex == PRESET_LOW ? "Low" : "High";

        Debug.Log($"Quality preset applied: {(presetIndex == PRESET_LOW ? "Low" : "High")} " +
                  $"(Level {(presetIndex == PRESET_LOW ? lowest : highest)})");
    }

    private void ShowMenuPanel()
    {
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}


