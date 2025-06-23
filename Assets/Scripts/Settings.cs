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

    // Índices internos para presets
    private const int PRESET_LOW = 0;
    private const int PRESET_HIGH = 1;

    private void Start()
    {
        // 1) Carregar preset salvo (0 = Low, 1 = High)
        int savedPreset = PlayerPrefs.GetInt("quality", PRESET_HIGH);

        // 2) Configurar opções do dropdown
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string> { "Low", "High" });

        // 3) Ligação de callbacks
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityPresetChanged);
        eraseProgressButton.onClick.AddListener(OnEraseProgressClicked);
        applyButton.onClick.AddListener(OnApplyClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);

        // 4) Aplicar valores iniciais
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1f);
        OnVolumeChanged(volumeSlider.value);

        qualityDropdown.value = savedPreset;
        ApplyQualityPreset(savedPreset);
    }

    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }

    private void OnQualityPresetChanged(int presetIndex)
    {
        ApplyQualityPreset(presetIndex);
    }

    private void OnEraseProgressClicked()
    {
        LevelProgressManager.Instance.ResetProgress();
        Debug.Log("Progresso zerado!");
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
                  $"(Level { (presetIndex == PRESET_LOW ? lowest : highest) })");
    }

    private void ShowMenuPanel()
    {
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}


