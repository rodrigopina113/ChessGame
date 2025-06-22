using UnityEngine.Timeline;
using UnityEngine;  
using UnityEngine.Video;

[System.Serializable]
public class LevelData
{
    public string        levelName;
    public string        sceneName;
    public Sprite        previewSprite;
    public GameObject    planetPrefab;

    [Header("Cutscene")]
    public VideoClip  cutsceneClip;
}

