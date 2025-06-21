// LevelProgressManager.cs
using UnityEngine;
using System;

public class LevelProgressManager : MonoBehaviour
{
    public static LevelProgressManager Instance { get; private set; }
    public event Action<int> OnProgressUnlocked;

    private const string SaveKey = "HighestUnlockedLevel";
    private int highestUnlocked = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            highestUnlocked = PlayerPrefs.GetInt(SaveKey, 0);
        }
        else Destroy(gameObject);
    }

    public int GetHighestUnlocked() => highestUnlocked;

    /// <summary>
    /// Unlock up through this index (0-based). Fires OnProgressUnlocked if new.
    /// </summary>
    public void UnlockLevel(int index)
    {
        if (index > highestUnlocked && index < 1000) // sanity cap
        {
            highestUnlocked = index;
            PlayerPrefs.SetInt(SaveKey, highestUnlocked);
            PlayerPrefs.Save();
            OnProgressUnlocked?.Invoke(highestUnlocked);
        }
    }
}
