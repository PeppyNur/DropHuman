using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("SCRIPTABLE OBJECTS")]
    public UIEventSO uiEventSO;
    [SerializeField] private GameManagerSO gameManagerSO;

    [Header("SCRIPTS")]
    public GameManager gameManager;
    public MapManager mapManager;

    [Header("MENU PANEL COMPONENTS")]
    //Home Panel
    public GameObject homePanelUIObject;
    public Button playButton;
    public Button restartButton;
    public TextMeshProUGUI[] homeLevelTexts;
    public TextMeshProUGUI lifeCountText;
    public TextMeshProUGUI lifeCountdownText;
    public TextMeshProUGUI coinCountText;
    public GameObject coinCountObject; // gerekli her yerde aktif edilecek.
    public GameObject lifeObject; // gerekli her yerde aktif edilecek.

    //Settings Panel
    public Toggle vibrationToggle;
    public Toggle soundToggle;

    [Header("WIN/LOSE PANEL")]
    public GameObject loseUIObject;
    public GameObject winUIObject;
    public GameObject inGamePanel;
    public GameObject revivePanel;
    public GameObject reviveButton;
    public TextMeshProUGUI rewardCoinText;
    public TextMeshProUGUI reviveCoinText;
    public TextMeshProUGUI reviveTimeText;

    [Header("IN GAME PANEL")]
    //In game panel
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelCountdownTimerText;
    public TextMeshProUGUI frozeFeatureText;
    public TextMeshProUGUI bombFeatureText;
    public TextMeshProUGUI magnetFeatureText;


    #region UNITY METHODS
    private void Start()
    {
        HomeLevelTextUpdater();
        UpdateLevelText();
        UpdateCoinUI();


        if (gameManagerSO.currentLife == 0)
        {
            playButton.interactable = false;
        }
    }

    private void Update()
    {
        
    }
    private void OnEnable()
    {
        uiEventSO.EnableWinUIEvent.AddListener(EnableWinUIMethod);
        uiEventSO.EnableLoseUIEvent.AddListener(EnableLoseUIMethod);
        uiEventSO.DisableWinUIEvent.AddListener(DisableWinUIMethod);
        uiEventSO.DisableLoseUIEvent.AddListener(DisableLoseUIMethod);
        uiEventSO.EnableReviveUIEvent.AddListener(EnableReviveUIMethod);
        uiEventSO.DisableReviveUIEvent.AddListener(DisableReviveUIMethod);
    }

    private void OnDisable()
    {
        uiEventSO.EnableWinUIEvent.RemoveListener(EnableWinUIMethod);
        uiEventSO.EnableLoseUIEvent.RemoveListener(EnableLoseUIMethod);
        uiEventSO.DisableWinUIEvent.RemoveListener(DisableWinUIMethod);
        uiEventSO.DisableLoseUIEvent.RemoveListener(DisableLoseUIMethod);
        uiEventSO.EnableReviveUIEvent.RemoveListener(EnableReviveUIMethod);
        uiEventSO.DisableReviveUIEvent.RemoveListener(DisableReviveUIMethod);
    }
    #endregion

    #region UI CALLBACK METHODS

    //HOME
    //Home panelinde görseldeki level textlerinin güncellenmesi
    public void HomeLevelTextUpdater()
    {
        for (int i = 0; homeLevelTexts.Length > i; i++)
        {
            homeLevelTexts[i].text = (gameManagerSO.currentLevel + 1 + i).ToString();
        }
    }

    //LifeCount text güncellemeleri
    public void UpdateLifeUI(int currentLife, int maxLife)
    {
        if (lifeCountText != null)
        {
            lifeCountText.text = currentLife.ToString();
        }
    }

    //Life Countdown
    public void UpdateLifeTimerUI(float timeLeft)
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);

        if (lifeCountdownText != null)
        {
            lifeCountdownText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
        }
    }

    //Can fullenince gösterilecek 
    public void SetFullLifeText()
    {
        if (lifeCountdownText != null)
        {
            lifeCountdownText.text = "Full!";
        }
    }

    //Coin uı günceller
    //CoinCount gameObjesini gerekli yerlerde göstermek için kullanılır
    //Lose revive,win
    public void UpdateCoinUI()
    {
        coinCountText.text = gameManagerSO.currentCoin.ToString();
    }

    //Timer text güncelleme
    public void UpdateTimerText()
    {
        if (levelCountdownTimerText != null)
        {
            int minutes = Mathf.FloorToInt(gameManagerSO.currentLevelTimer / 60);
            int seconds = Mathf.FloorToInt(gameManagerSO.currentLevelTimer % 60);

            levelCountdownTimerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);

            // Timer kırmızıya dön (son 10 saniye)
            if (gameManagerSO.currentLevelTimer <= 10f)
            {
                levelCountdownTimerText.color = Color.red;
            }
            else
            {
                levelCountdownTimerText.color = Color.white;
            }
        }
    }

    //Level text güncelleme
    public void UpdateLevelText()
    {
        int level = gameManagerSO.currentLevel + 1;
        levelText.text = level.ToString();
    }

    //lose can gösterilsin //Lose kısmında direkt çağır 
    public void ShowLifeUIObject()
    {
        lifeObject.SetActive(true);
    }

    //Can azalt 
    public void DecreaseLifeCount()
    {
        if (gameManagerSO.currentLife > 0)
        {
            gameManagerSO.currentLife--;
            lifeCountText.text = gameManagerSO.currentLife.ToString();
        }
    }

    //vibration ve sound kaydetmesi ve ona göre çağırılması 

    //WIN UI HELPER METHODS
    private int CalculateAndAddReward()
    {
        int rewardAmount = 0;

        switch (gameManagerSO.currentLevelType)
        {
            case LevelType.Training:
                rewardAmount = gameManagerSO.trainingLevelCoin;
                break;
            case LevelType.Normal:
                rewardAmount = gameManagerSO.normalLevelCoin;
                break;
            case LevelType.Hard:
                rewardAmount = gameManagerSO.hardLevelCoin;
                break;
        }

        gameManagerSO.currentCoin += rewardAmount;
        return rewardAmount;
    }

    private void UpdateWinUI(int calculatedReward)
    {
        if (rewardCoinText != null)
        {
            rewardCoinText.text = calculatedReward.ToString();
        }
        UpdateCoinUI();
    }

    //LOSE UI HELPER METHODS
    //WIN UI METHODS
    public void EnableWinUIMethod()
    {
        if (gameManagerSO.isGameWin)
        {
            coinCountObject.SetActive(true);
            winUIObject.SetActive(true);
            inGamePanel.SetActive(false);
            loseUIObject.SetActive(false);
            revivePanel.SetActive(false);

            gameManagerSO.currentLevel++;
            gameManagerSO.SetTimerByLevelType();

            HomeLevelTextUpdater();
            UpdateLevelText();
            UpdateTimerText();

            int reward = CalculateAndAddReward();
            UpdateWinUI(reward);

            gameManagerSO.isGameStart = false;
        }
    }

    public void DisableWinUIMethod()
    {
        winUIObject.SetActive(false);
    }

    //LOSE UI METHODS
    public void EnableLoseUIMethod()
    {
            loseUIObject.SetActive(true);
            inGamePanel.SetActive(false);
            revivePanel.SetActive(false);
            gameManagerSO.isGameStart = false;
    }

    public void DisableLoseUIMethod()
    {
        loseUIObject.SetActive(false);
    }

    //REVIVE UI METHODS
    public void EnableReviveUIMethod()
    {
        revivePanel.SetActive(true);
        inGamePanel.SetActive(false);
        loseUIObject.SetActive(false);
        reviveCoinText.text = gameManagerSO.reviveLevelTimer.ToString();

        ShowLifeUIObject();
        UpdateCoinUI();

        gameManagerSO.isGameStart = false;
    }

    public void DisableReviveUIMethod()
    {
        revivePanel.SetActive(false);
    }

    public void UseLifeToRevive()
    {
        if (gameManagerSO.currentLife > 0)
        {
            DecreaseLifeCount();
            DisableReviveUIMethod();
            RestartCurrentLevel();
        }
        if (gameManagerSO.currentLife == 0)
        {
            restartButton.interactable = false;

        }
    }

    public void UseCoinToRevive()
    {
        if (gameManagerSO.currentCoin >= gameManagerSO.reviveCoin)
        {
            gameManagerSO.currentCoin -= gameManagerSO.reviveCoin;
            DisableReviveUIMethod();
        }
    }

    private void RestartCurrentLevel()
    {
        gameManagerSO.isGameStart = true;
        gameManagerSO.isGameOver = false;
        gameManagerSO.isGameWin = false;
        gameManagerSO.SetTimerByLevelType();
        DisableLoseUIMethod();
        inGamePanel.SetActive(true);
        UpdateTimerText();
        mapManager.LoadSavedLevel();
    }

    public void UpdateReviveUI()
    {
        reviveCoinText.text = gameManagerSO.reviveCoin.ToString();
    }

    public void PauseGame()
    {
        Time.timeScale=0f;
        gameManagerSO.isGamePause = true;
    }
    public void ContinueGame()
    {
        Time.timeScale = 1f;
        gameManagerSO.isGamePause = false;
    }

    #endregion
}