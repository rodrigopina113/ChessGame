using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string levelName;       // e.g. "Sand Kingdom"
    public string sceneName;       // e.g. "SandScene"
    public Sprite previewSprite;   // screenshot
    public GameObject planetPrefab;// mini-planet model
}
