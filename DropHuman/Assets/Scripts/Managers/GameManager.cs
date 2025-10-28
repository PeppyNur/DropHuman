using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class GameManager : MonoBehaviour
{
    #region VARIABLES
    public GameManagerSO gameManagerSO;
    public UIEventSO uiEventSO;
    public List<GameObject> blocks;
    public UIManager uiManager;
    #endregion

    #region UNITY METHODS
    private void Start()
    {
        LoadData();
        gameManagerSO.isGameStart = false;
        gameManagerSO.isGameWin = false;
        gameManagerSO.isGamePause = false;
        gameManagerSO.isGameOver = false;
        gameManagerSO.SetTimerByLevelType();
    }
    private void Update()
    {
        StartLevelTimer();
        uiManager.DisablePlayButton();
        if (gameManagerSO.isGamePause)
        {
            uiManager.PauseGame();
        }
        else
        {
            uiManager.ContinueGame();
        }
        
    }
    #endregion
    #region SETTINGS


    #endregion

    #region GAME METHODS
    void FindAllBlocks()
    {
        GameObject[] foundBlocks = GameObject.FindGameObjectsWithTag("Block");
        blocks.Clear(); 
        blocks.AddRange(foundBlocks);
    }
    void CheckGameStatus()
    {
        if (blocks.Count == 0)
        {
            gameManagerSO.isGameWin = true;
            gameManagerSO.isGameOver = false;
            gameManagerSO.isGameStart = false;

            uiEventSO.EnableWinUIEvent.Invoke();
        }
        // Kazanmadıysa ve süre bittiyse kaybetme/canlanma durumunu kontrol et
        else if (gameManagerSO.currentLevelTimer <= 0)
        {
            gameManagerSO.isGameOver = true;
            gameManagerSO.isGameWin = false;
            gameManagerSO.isGameStart = false;
            uiEventSO.EnableLoseUIEvent.Invoke();
            uiManager.PauseGame();
        }
    }
    public void StartLevelTimer()
    {
        if (gameManagerSO.isGameStart && !gameManagerSO.isGamePause)
        {
            if(!gameManagerSO.isFrozenUsed)
            {
                if (gameManagerSO.currentLevelTimer > 0)
                {
                    gameManagerSO.currentLevelTimer -= Time.deltaTime;
                    uiManager.UpdateTimerText();
                }
                else
                {
                    gameManagerSO.currentLevelTimer = 0; // Sayacı 0'da sabitle
                }
                // --- BİTTİ ---

                FindAllBlocks();

                CheckGameStatus();
            }
            
        }

    }
    public void AddPlayTime()
    {
        gameManagerSO.currentLevelTimer = gameManagerSO.addReviveTimer;
        gameManagerSO.isGameStart = true;
        gameManagerSO.isGamePause = false;
        gameManagerSO.isGameWin = false;
        gameManagerSO.isGameOver = false;
    }
    #endregion

    #region DATA SETTINGS
    private void OnApplicationQuit()
    {
        SaveExitTime();
        SaveData();

        if (gameManagerSO.isGameOver || gameManagerSO.isGameStart || gameManagerSO.isGamePause)
        {
            gameManagerSO.currentLife--;
        }
    }
    public void SaveExitTime()
    {
        string isoNow = DateTime.UtcNow.ToString("o"); // ISO-8601, UTC
        PlayerPrefs.SetString("LastExitTime", isoNow);
        PlayerPrefs.Save();
    }
    public float GetOfflineTimePassed()
    {
        if (!PlayerPrefs.HasKey("LastExitTime"))
            return 0f;

        string stored = PlayerPrefs.GetString("LastExitTime");
        DateTime lastExitUtc = DateTime.Parse(stored, null, System.Globalization.DateTimeStyles.RoundtripKind);
        DateTime nowUtc = DateTime.UtcNow;
        TimeSpan timePassed = nowUtc - lastExitUtc;


        return (float)timePassed.TotalSeconds;
    }

    public void SaveData()
    {
        // ==================== LIFE SETTINGS ====================
        PlayerPrefs.SetInt("currentLife",gameManagerSO.currentLife);

        // ==================== MAP SETTINGS ====================
        PlayerPrefs.SetString("currentMapName",gameManagerSO.currentMap.name);
        PlayerPrefs.SetInt("currentLevel",gameManagerSO.currentLevel);

        // ==================== COIN SETTINGS ====================
        PlayerPrefs.SetInt("currentCoin",gameManagerSO.currentCoin);

        // ==================== GAME STATE ====================
        PlayerPrefs.SetInt("isVibrating",gameManagerSO.isVibrating ?  1 : 0);
        PlayerPrefs.SetInt("isSoundOn", gameManagerSO.isSoundOn ? 1 : 0);

        // ==================== FEATURE SETTINGS ====================
        PlayerPrefs.SetInt("freezeCount", gameManagerSO.freezeCount);
        PlayerPrefs.SetInt("bombCount", gameManagerSO.bombCount);
        PlayerPrefs.SetInt("magnetCount",gameManagerSO.magnetCount);

        PlayerPrefs.SetInt("freezeGiftLevel",gameManagerSO.freezeGiftLevel);
        PlayerPrefs.SetInt("bombGiftLevel",gameManagerSO.bombGiftLevel);
        PlayerPrefs.SetInt("magnetGiftLevel", gameManagerSO.bombGiftLevel);

        PlayerPrefs.Save();
        Debug.Log("DATA SAVED");

    }

    public void LoadData()
    {
        // ==================== LIFE SETTINGS ====================
        gameManagerSO.currentLife = PlayerPrefs.GetInt("currentLife",gameManagerSO.currentLife);

        // ==================== COIN SETTINGS ====================
        gameManagerSO.currentCoin = PlayerPrefs.GetInt("currentCoin", gameManagerSO.currentCoin);

        // ==================== GAME STATE ====================
        gameManagerSO.isVibrating = PlayerPrefs.GetInt("isVibrating", 1)==1;
        gameManagerSO.isSoundOn = PlayerPrefs.GetInt("isSoundOn", 1) == 1;

        // ==================== FEATURE SETTINGS ====================
        gameManagerSO.freezeCount = PlayerPrefs.GetInt("freezeCount", gameManagerSO.freezeCount);
        gameManagerSO.bombCount = PlayerPrefs.GetInt("bombCount",gameManagerSO.bombCount);
        gameManagerSO.magnetCount = PlayerPrefs.GetInt("magnetCount",gameManagerSO.magnetCount);

        // ==================== MAP SETTINGS ====================

        string currentMapName = PlayerPrefs.GetString("currentMapName", "");
        if(!string.IsNullOrEmpty(currentMapName))
        {
            List<GameObject> allMaps = new List<GameObject>();
            allMaps.AddRange(gameManagerSO.trainingLevelMaps);
            allMaps.AddRange(gameManagerSO.normalLevelMaps);
            allMaps.AddRange(gameManagerSO.hardLevelMaps);

            foreach(GameObject map in allMaps)
            {
                if(map.name == currentMapName)
                {
                    gameManagerSO.currentMap = map;
                    Debug.Log($"HARİTA YÜKLENDİ: {currentMapName}");
                    break;
                }
            }
        }
        else
        {
            Debug.Log("KAYITLI HARİTA BULUNAMADI VARSAYILAN KULLANILACAK");
        }
        if (gameManagerSO.currentLevel == 0)
        {
            gameManagerSO.currentMap = null;
        }
    }

    #endregion
}