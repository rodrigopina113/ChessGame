using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LevelProgressManager : MonoBehaviour
{
    public static LevelProgressManager Instance { get; private set; }
    public event Action<int> OnProgressUnlocked;

    public event Action<int> OnSkinUnlocked;
    private const string SkinSaveKey = "HighestUnlockedSkin";
    private int highestUnlockedSkin = 0;

    private const string SaveKey = "HighestUnlockedLevel";
    private int highestUnlocked = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            highestUnlocked = PlayerPrefs.GetInt(SaveKey, 0);
            highestUnlockedSkin = PlayerPrefs.GetInt(SkinSaveKey, 0);

            // Subscribe to scene-loaded so we can unpause when menus load:
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If you have specific menu/selector scene names, you can check:
        if (scene.name == "Menu" || scene.name == "LevelSelect")
        {
            Time.timeScale = 1f;
            Debug.Log($"[ProgressManager] Reset timeScale on loading {scene.name}");
        }
        // Or, if you’d rather always unpause on any load:
        // Time.timeScale = 1f;
    }

    public int GetHighestUnlocked() => highestUnlocked;
    public int GetHighestUnlockedSkin() => highestUnlockedSkin;

    public void UnlockLevel(int index)
    {
        if (index > highestUnlocked && index < 1000)
        {
            highestUnlocked = index;
            PlayerPrefs.SetInt(SaveKey, highestUnlocked);
            PlayerPrefs.Save();
            OnProgressUnlocked?.Invoke(highestUnlocked);
            UnlockSkin(index);
        }
    }

    public void UnlockSkin(int index)
    {
        if (index > highestUnlockedSkin)
        {
            highestUnlockedSkin = index;
            PlayerPrefs.SetInt(SkinSaveKey, highestUnlockedSkin);
            PlayerPrefs.Save();
            OnSkinUnlocked?.Invoke(highestUnlockedSkin);
        }
    }

    public void ResetProgress()
    {
        highestUnlocked = 0;
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
        OnProgressUnlocked?.Invoke(highestUnlocked);
    }
}
