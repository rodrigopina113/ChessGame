using UnityEngine;   // necessário para Sprite, GameObject e TooltipAttribute

[System.Serializable]
public class LevelData {
    public string    levelName;
    public string    sceneName;
    public Sprite    previewSprite;
    public GameObject planetPrefab;

    [Tooltip("Nome do ficheiro em StreamingAssets/WebGLVideos (ex: 'cutscene1.webm')")]
    public string    cutsceneFileName;
}
