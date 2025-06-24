using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider volumeSlider;
    public TMP_Dropdown qualityDropdown;
    public TMP_Text qualityLabel;    // opcional, para mostrar “Low”, “Medium” ou "High" na UI
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
    private const int PRESET_MEDIUM = 1;
    private const int PRESET_HIGH = 2;

    private void Start()
    {
        // Carrega preset salvo (0 = Low, 1 = Medium, 2 = High)
        int savedPreset = PlayerPrefs.GetInt("quality", PRESET_MEDIUM);

        // Configurar opções do dropdown
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string> { "Low", "Medium", "High" });

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

        qualityDropdown.value = Mathf.Clamp(savedPreset, PRESET_LOW, PRESET_HIGH);
        ApplyQualityPreset(qualityDropdown.value);
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
        confirmPanel.SetActive(true);
    }

    private void OnConfirmYesClicked()
    {
        LevelProgressManager.Instance.ResetProgress();
        Debug.Log("Progresso zerado!");
        confirmPanel.SetActive(false);
    }

    private void OnConfirmNoClicked()
    {
        confirmPanel.SetActive(false);
    }

    private void OnApplyClicked()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
        PlayerPrefs.SetInt("quality", qualityDropdown.value);
        PlayerPrefs.Save();
        ShowMenuPanel();
    }

    private void OnCancelClicked()
    {
        ShowMenuPanel();
    }

    #endregion

    private void ApplyQualityPreset(int presetIndex)
    {
        // Mapeia o índice do preset ao índice de QualitySettings.names
        string[] names = QualitySettings.names;
        int targetLevel;
        switch (presetIndex)
        {
            case PRESET_LOW:
                targetLevel = 0;
                break;
            case PRESET_MEDIUM:
                // Tenta encontrar "Medium" nos perfis, senão usa meio do array
                int midIndex = Array.IndexOf(names, "PC");
                targetLevel = midIndex >= 0 ? midIndex : names.Length / 2;
                break;
            case PRESET_HIGH:
            default:
                targetLevel = names.Length - 1;
                break;
        }

        QualitySettings.SetQualityLevel(targetLevel);

        if (qualityLabel != null)
            qualityLabel.text = qualityDropdown.options[presetIndex].text;

        Debug.Log($"Quality preset applied: {qualityDropdown.options[presetIndex].text} (Level {targetLevel})");
    }

    private void ShowMenuPanel()
    {
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}





