using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("UI References")]
    public Slider volumeSlider;
    public TMP_Dropdown qualityDropdown;
    public TMP_Text qualityLabel;
    public GameObject settingsPanel;
    public GameObject menuPanel;

    [Header("Buttons")]
    public Button applyButton;
    public Button cancelButton;
    public Button eraseProgressButton;

    [Header("Confirm Dialog")]
    public GameObject confirmPanel;
    public Button confirmYesButton;
    public Button confirmNoButton;

    private const int PRESET_LOW = 0;
    private const int PRESET_MEDIUM = 1;
    private const int PRESET_HIGH   = 2;

    private void Awake()
    {
        // Singleton + manter entre cenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // --- Carregar e aplicar imediatamente as prefs salvas ---

        // Volume
        float savedVol = PlayerPrefs.GetFloat(PREF_VOLUME, 1f);
        volumeSlider.value = savedVol;
        AudioListener.volume = savedVol;

        // Qualidade
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string> { "Low", "Medium", "High" });
        // na primeira vez, grava PRESET_HIGH (2) se não existir
        if (!PlayerPrefs.HasKey(PREF_QUALITY))
        {
            PlayerPrefs.SetInt(PREF_QUALITY, PRESET_HIGH);
            PlayerPrefs.Save();
        }
        int savedQuality = PlayerPrefs.GetInt(PREF_QUALITY);
        qualityDropdown.value = Mathf.Clamp(savedQuality, PRESET_LOW, PRESET_HIGH);
        ApplyQualityPreset(qualityDropdown.value);

        // Skin
        bool savedClassic = PlayerPrefs.GetInt(PREF_WHITE_SKIN, 1) == 1;
        skinToggle.isOn = savedClassic;
    }

    private void Start()
    {
        int savedPreset = PlayerPrefs.GetInt("quality", PRESET_MEDIUM);

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string> { "Low", "Medium", "High" });

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityPresetChanged);
        eraseProgressButton.onClick.AddListener(ShowConfirmPanel);
        applyButton.onClick.AddListener(OnApplyClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);

        confirmPanel.SetActive(false);
        confirmYesButton.onClick.AddListener(OnConfirmYesClicked);
        confirmNoButton.onClick.AddListener(OnConfirmNoClicked);

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
        confirmPanel.SetActive(false);
    }

    private void ApplyQualityPreset(int presetIndex)
    {
        string[] names = QualitySettings.names;
        int level;
        switch (presetIndex)
        {
            case PRESET_LOW:
                level = 0;
                break;
            case PRESET_MEDIUM:
                int mid = Array.IndexOf(names, "PC");
                level = mid >= 0 ? mid : names.Length / 2;
                break;
            default:
                level = names.Length - 1;
                break;
        }
        QualitySettings.SetQualityLevel(level);
        if (qualityLabel != null)
            qualityLabel.text = qualityDropdown.options[presetIndex].text;
    }

    private void OnApplyClicked()
    {
        // grava prefs
        PlayerPrefs.SetFloat(PREF_VOLUME, volumeSlider.value);
        PlayerPrefs.SetInt(PREF_QUALITY, qualityDropdown.value);
        PlayerPrefs.SetInt(PREF_WHITE_SKIN, skinToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();

        // aplica a qualidade
        ApplyQualityPreset(qualityDropdown.value);

        // fecha painel
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    private void OnCancelClicked()
    {
        // repõe UI aos valores salvos
        volumeSlider.value = PlayerPrefs.GetFloat(PREF_VOLUME, 1f);
        qualityDropdown.value = PlayerPrefs.GetInt(PREF_QUALITY, PRESET_HIGH);
        skinToggle.isOn = PlayerPrefs.GetInt(PREF_WHITE_SKIN, 1) == 1;
    }

    #endregion



    private void ShowMenuPanel()
    {
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}












