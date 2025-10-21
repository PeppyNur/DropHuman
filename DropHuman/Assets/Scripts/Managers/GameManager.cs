using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    [SerializeField] private bool reviveClicked;
    #endregion

    private void Start()
    {
        gameManagerSO.isGameStart = false;
        gameManagerSO.isGameWin = false;
        gameManagerSO.isGameOver = false;
        gameManagerSO.SetTimerByLevelType();

    }
    private void Update()
    {
        StartLevelTimer();
        
    }

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

    public void StartLevelTimer()
    {
        if (gameManagerSO.isGameStart && !gameManagerSO.isGamePause)
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

        if(reviveClicked==true)
        {
            if (gameManagerSO.isGameStart && !gameManagerSO.isGamePause)
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
        reviveClicked=true;
        gameManagerSO.currentLevelTimer = gameManagerSO.trainingLevelTimer;
        gameManagerSO.isGameStart = true;
        gameManagerSO.isGamePause = false;
        gameManagerSO.isGameWin = false;
        gameManagerSO.isGameOver = false;
        Debug.Log("BUTONA TIKLANDI");
    }
}