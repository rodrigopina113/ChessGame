using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingsManager : MonoBehaviour
{
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
    private const int PRESET_HIGH = 2;

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
        string[] names = QualitySettings.names;
        int targetLevel;
        switch (presetIndex)
        {
            case PRESET_LOW:
                targetLevel = 0;
                break;
            case PRESET_MEDIUM:
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
    }

    private void ShowMenuPanel()
    {
        settingsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}





