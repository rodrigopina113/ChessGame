using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI References")]
    public Slider volumeSlider;
    public Dropdown qualityDropdown;
    public GameObject settingsPanel;

    private void Start()
    {
        // Carrega valores salvos
        volumeSlider.value      = PlayerPrefs.GetFloat("volume", 1f);
        qualityDropdown.value   = PlayerPrefs.GetInt("quality", QualitySettings.GetQualityLevel());

        // Aplica imediatamente
        ApplyAudio(volumeSlider.value);
        ApplyQuality(qualityDropdown.value);
    }

    #region Callbacks

    public void OnVolumeChanged(float value)
    {
        ApplyAudio(value);
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

