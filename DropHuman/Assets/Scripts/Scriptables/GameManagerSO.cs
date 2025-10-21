using System.Collections.Generic;
using UnityEngine;

public enum LevelType
{
    Training,
    Normal,
    Hard
}

[CreateAssetMenu(fileName = "GameManagerSO", menuName = "Scriptable Objects/GameManagerSO")]
public class GameManagerSO : ScriptableObject
{
    // ==================== LEVEL SETTINGS ====================
    [Header("LEVEL SETTINGS")]
    public LevelType currentLevelType;
    public int currentLevel;
    public int savedLevel;

    [Space(5)]
    public float trainingLevelTimer;
    public float normalLevelTimer;
    public float hardLevelTimer;
    public float currentLevelTimer;
    public float reviveLevelTimer;


    // ==================== LIFE SETTINGS ====================
    [Header("LIFE SETTINGS")]
    public float lifeCountdown;
    public int currentLife;
    public int maxLife;


    // ==================== MAP SETTINGS ====================
    [Header("MAP SETTINGS")]
    public GameObject currentMap;
    public List<GameObject> trainingLevelMaps = new List<GameObject>();
    public List<GameObject> normalLevelMaps = new List<GameObject>();
    public List<GameObject> hardLevelMaps = new List<GameObject>();


    // ==================== COIN SETTINGS ====================
    [Header("COIN SETTINGS")]
    public int currentCoin;
    public int trainingLevelCoin;
    public int normalLevelCoin;
    public int hardLevelCoin;
    public int reviveCoin;


    // ==================== GAME STATE ====================
    [Header("GAME STATE")]
    public bool isGameOver;
    public bool isGameWin;
    public bool isGameStart;
    public bool isGamePause;

    [Space(5)]
    public bool isVibrating;
    public bool isSoundOn;


    // ==================== TIMER MANAGEMENT ====================
    #region Timer Management
    public void SetTimerByLevelType()
    {
        switch (currentLevelType)
        {
            case LevelType.Training:
                currentLevelTimer = trainingLevelTimer;
                break;

            case LevelType.Normal:
                currentLevelTimer = normalLevelTimer;
                break;

            case LevelType.Hard:
                currentLevelTimer = hardLevelTimer;
                break;
        }
    }
    #endregion
}
