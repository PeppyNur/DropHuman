using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class UIManager : MonoBehaviour
{
    #region VARIABLES
    [Header("SCRIPTABLE OBJECTS")]
    public UIEventSO uiEventSO;
    [SerializeField] private GameManagerSO gameManagerSO;

    [Header("SCRIPTS")]
    public GameManager gameManager;
    public MapManager mapManager;

    [Header("HOME PANEL COMPONENTS")]
    public GameObject homePanelUIObject;

    // Buttons
    [Space(5)]
    public Button playButton;
    public Button restartButton;

    // Texts
    [Space(5)]
    public TextMeshProUGUI[] homeLevelTexts;
    public TextMeshProUGUI lifeCountText;
    public TextMeshProUGUI lifeCountdownText;
    public TextMeshProUGUI coinCountText;

    // UI Objects
    [Space(5)]
    public GameObject coinObject; 
    public GameObject lifeObject;      

    [Header("SETTINGS PANEL COMPONENTS")]
    public Toggle vibrationToggle;
    public Toggle soundToggle;
    public AudioClip clickSound;
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioMixer mainAudioMixer;

    [Header("WIN / LOSE PANELS")]
    public GameObject loseUIObject;
    public GameObject winUIObject;

    // In-Game Related Panels
    [Space(5)]
    public GameObject inGamePanel;
    public GameObject revivePanel;

    [Space(5)]
    public Button reviveButton;
    public TextMeshProUGUI rewardCoinText;
    public TextMeshProUGUI reviveCoinText;
    public TextMeshProUGUI reviveTimerText;

    [Header("IN-GAME PANEL COMPONENTS")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelCountdownTimerText;

    [Header("FEATURE SETTINGS")]
    public GameObject giftPanel;
    public GameObject bombGiftPanel;
    public TextMeshProUGUI bombGiftText;
    public Button bombButton;

    [Space(5)]
    public GameObject magnetGiftPanel;
    public TextMeshProUGUI magnetGiftText;
    public Button magnetButton;


    [Space(5)]
    public GameObject frozeGiftPanel;
    public TextMeshProUGUI freezeGiftText;
    public Button frozenButton;

    #endregion

    #region UNITY METHODS
    private void Start()
    {
        HomeLevelTextUpdater();
        UpdateLevelText();
        UpdateCoinUI();

        DisablePlayButton();
        InitializeToggles();
        AddToggleListeners();

        gameManager.LoadData();

    }
    private void Update()
    {
        ButtonActivate();
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

    #region SETTINGS METHODS
    private void InitializeToggles()
    {
        gameManagerSO.isSoundOn = PlayerPrefs.GetInt("isSoundOn", 1) == 1; // Varsayılan: Açık
        gameManagerSO.isVibrating = PlayerPrefs.GetInt("isVibrating", 1) == 1;

        soundToggle.isOn = gameManagerSO.isSoundOn;
        vibrationToggle.isOn = gameManagerSO.isVibrating;

        // Oyun başladığında ses seviyelerini ayarla
        SetMixerVolume(gameManagerSO.isSoundOn);
    }

    private void AddToggleListeners()
    {
        soundToggle.onValueChanged.AddListener((bool value) =>
        {
            gameManagerSO.isSoundOn = value;
            PlayerPrefs.SetInt("isSoundOn", value ? 1 : 0);
            PlayerPrefs.Save();

            // YENİ: Mixer ses seviyelerini ayarla
            SetMixerVolume(value);

        });

        vibrationToggle.onValueChanged.AddListener((bool value) =>
        {
            gameManagerSO.isVibrating = value;
            PlayerPrefs.SetInt("isVibrating", value ? 1 : 0);
            PlayerPrefs.Save();

        });
    }

    private void SetMixerVolume(bool isSoundOn)
    {
        if (mainAudioMixer == null) return;

        float volume = isSoundOn ? 0f : -80f;

        mainAudioMixer.SetFloat("MusicVolume", volume);
        mainAudioMixer.SetFloat("SFXVolume", volume);
    }
    public void VibrateOnce()
    {
        if (gameManagerSO.isVibrating)
        {
            Handheld.Vibrate();
        }
    }

    public void PlayClickSounds()
    {
        if(gameManagerSO.isSoundOn && clickSound!=null && sfxSource!=null)
        {
            sfxSource.PlayOneShot(clickSound);
        }
    }
    #endregion

    #region HOME UI METHODS

    // Ana menüde gösterilen level textlerini günceller
    public void HomeLevelTextUpdater()
    {
        for (int i = 0; i < homeLevelTexts.Length; i++)
            homeLevelTexts[i].text = (gameManagerSO.currentLevel + 1 + i).ToString();
    }

    // Can sayısını ekrana yansıtır
    public void UpdateLifeUI(int currentLife, int maxLife)
    {
        if (lifeCountText != null)
            lifeCountText.text = currentLife.ToString();
    }

    // Can dolum süresini günceller
    public void UpdateLifeTimerUI(float timeLeft)
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);

        if (lifeCountdownText != null)
            lifeCountdownText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }

    // Canlar tamamen dolduğunda “Full!” yazar
    public void SetFullLifeText()
    {
        if (lifeCountdownText != null)
            lifeCountdownText.text = "Full!";
    }

    // Coin miktarını günceller
    public void UpdateCoinUI()
    {
        coinCountText.text = gameManagerSO.currentCoin.ToString();
    }

    public void GiftPanel()
    {
        gameManagerSO.isGamePause = !giftPanel;

        if (gameManagerSO.currentLevel == gameManagerSO.freezeGiftLevel || gameManagerSO.currentLevel == gameManagerSO.bombGiftLevel || gameManagerSO.currentLevel == gameManagerSO.magnetGiftLevel)
        {
            giftPanel.SetActive(true);
            gameManagerSO.isGamePause = true;

            if (gameManagerSO.currentLevel == gameManagerSO.freezeGiftLevel && gameManagerSO.freezeGiftCount > 0 && gameManagerSO.isGameWin)
            {
                freezeGiftText.text = gameManagerSO.freezeGiftCount.ToString();
                gameManagerSO.freezeCount += gameManagerSO.freezeGiftCount;

                frozeGiftPanel.SetActive(true);
                gameManagerSO.IsFreezeFeatureGifted = true;
            }
            else
            {
                frozeGiftPanel.SetActive(false);
            }

            if (gameManagerSO.currentLevel == gameManagerSO.bombGiftLevel && gameManagerSO.bombGiftCount > 0 && gameManagerSO.isGameWin)
            {
                bombGiftText.text = gameManagerSO.bombGiftCount.ToString();
                gameManagerSO.bombCount += gameManagerSO.bombGiftCount;

                bombGiftPanel.SetActive(true);
                gameManagerSO.isBombFeatureGifted = true;
            }
            else
            {
                bombGiftPanel.SetActive(false);
            }

            if (gameManagerSO.currentLevel == gameManagerSO.magnetGiftLevel && gameManagerSO.magnetGiftCount > 0 && gameManagerSO.isGameWin)
            {
                magnetGiftText.text = gameManagerSO.magnetGiftCount.ToString();
                gameManagerSO.magnetCount += gameManagerSO.magnetGiftCount;

                magnetGiftPanel.SetActive(true);
                gameManagerSO.isMagnetFeatureGifted = true;
            }
            else
            {
                magnetGiftPanel.SetActive(false);
            }

        }

    }

    public void ButtonActivate()
    {
        //froze
        if (gameManagerSO.currentLevel >= gameManagerSO.freezeGiftLevel)
        {
            frozenButton.interactable = true;
        }
        else
        {
            frozenButton.interactable = false;
        }

        //bomb
        if (gameManagerSO.currentLevel >= gameManagerSO.bombGiftLevel)
        {
            bombButton.interactable = true;
        }
        else
        {
            bombButton.interactable = false;
        }

        //magnet
        if (gameManagerSO.currentLevel >= gameManagerSO.magnetGiftLevel)
        {
            magnetButton.interactable = true;
        }
        else
        {
            magnetButton.interactable = false;
        }
    }

    public void DisablePlayButton()
    {
        if (gameManagerSO.currentLife == 0)
            playButton.interactable = false;
        else playButton.interactable = true;
    }

    #endregion


    #region IN-GAME UI METHODS

    // Level zamanlayıcısını ekranda günceller
    public void UpdateTimerText()
    {
        if (levelCountdownTimerText != null)
        {
            int minutes = Mathf.FloorToInt(gameManagerSO.currentLevelTimer / 60);
            int seconds = Mathf.FloorToInt(gameManagerSO.currentLevelTimer % 60);
            levelCountdownTimerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
            // Süre 10 saniyenin altına indiğinde kırmızıya döner
            levelCountdownTimerText.color = gameManagerSO.currentLevelTimer <= 10f ? Color.red : Color.white;
            
        }
    }

    // Mevcut level numarasını gösterir
    public void UpdateLevelText()
    {
        levelText.text = (gameManagerSO.currentLevel + 1).ToString();
    }

    // Bir can azaltır ve UI’ı günceller
    public void DecreaseLifeCount()
    {
        if (gameManagerSO.currentLife > 0)
        {
            gameManagerSO.currentLife--;
            lifeCountText.text = gameManagerSO.currentLife.ToString();
        }
    }

   

    #endregion


    #region WIN / REWARD METHODS

    // Kazanılan level zorluğuna göre ödül hesaplar ve coin ekler
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

    // Kazanılan ödülü UI'da gösterir
    private void UpdateWinUI(int calculatedReward)
    {
        if (rewardCoinText != null)
            rewardCoinText.text = calculatedReward.ToString();

        UpdateCoinUI();
    }

    #endregion


    #region REVIVE / RESTART METHODS

    // Can kullanarak yeniden doğmayı sağlar (butonda bağlı)
    public void UseLifeToRevive()
    {
        if (gameManagerSO.currentLife > 0)
        {
            DecreaseLifeCount();
            DisableReviveUIMethod();
            RestartCurrentLevel();
        }

        if (gameManagerSO.currentLife == 0)
            restartButton.interactable = false;
    }

    // Coin harcayarak yeniden doğmayı sağlar (butonda bağlı)
    public void UseCoinToRevive()
    {
        if (gameManagerSO.currentCoin >= gameManagerSO.reviveCoin)
        {
            gameManagerSO.currentCoin -= gameManagerSO.reviveCoin;
            inGamePanel.SetActive(true);
            gameManager.AddPlayTime();
            ContinueGame();
            DisableLoseUIMethod();
            DisableReviveUIMethod();
        }
        else
        {
            reviveButton.interactable = false;
            Debug.Log("YETERLİ PARA YOK");
        }

    }

    // Leveli baştan başlatır
    public void RestartCurrentLevel()
    {
        gameManagerSO.isGameOver = false;
        gameManagerSO.isGameWin = false;
        gameManagerSO.isGamePause = false;
        ContinueGame();
        gameManagerSO.SetTimerByLevelType();
        lifeObject.SetActive(false);
        inGamePanel.SetActive(true);
        mapManager.LoadSavedLevel();
    }

    // Revive panelindeki coin bilgisini butonla günceller
    public void UpdateReviveUI()
    {
        reviveCoinText.text = gameManagerSO.reviveCoin.ToString();
        reviveTimerText.text = "+"+gameManagerSO.addReviveTimer.ToString();
        if (gameManagerSO.currentCoin < gameManagerSO.reviveCoin)
        {
            reviveButton.interactable = false;
        }

    }

    #endregion


    #region GAME CONTROL METHODS

    // Oyunu duraklatır
    public void PauseGame()
    {
        Time.timeScale = 0f;
        gameManagerSO.isGamePause = true;
    }

    // Oyunu devam ettirir
    public void ContinueGame()
    {
        Time.timeScale = 1f;
        gameManagerSO.isGamePause = false;
    }

    #endregion


    #region UI ENABLE / DISABLE METHODS

    //WIN
    public void EnableWinUIMethod()
    {
        if (gameManagerSO.isGameWin)
        {
            coinObject.SetActive(true);
            winUIObject.SetActive(true);
            inGamePanel.SetActive(false);
            loseUIObject.SetActive(false);
            revivePanel.SetActive(false);

            gameManagerSO.currentLevel++;
            GiftPanel();
            gameManagerSO.SetTimerByLevelType();

            HomeLevelTextUpdater();
            UpdateLevelText();
            UpdateTimerText();

            int reward = CalculateAndAddReward();
            UpdateWinUI(reward);

            gameManagerSO.isGameStart = false;
            gameManagerSO.currentMap=null;
        }
    }

    public void DisableWinUIMethod()
    {
        winUIObject.SetActive(false);
        coinObject.SetActive(false);

    }

    //LOSE

    public void EnableLoseUIMethod()
    {
        loseUIObject.SetActive(true);
        lifeObject.SetActive(true);
        inGamePanel.SetActive(false);
        revivePanel.SetActive(false);
        gameManagerSO.isGameStart = false;

    }

    public void DisableLoseUIMethod()
    {
        lifeObject.SetActive(false);
        loseUIObject.SetActive(false);
    }

    //REVIVE
    public void EnableReviveUIMethod()
    {
        revivePanel.SetActive(true);
        inGamePanel.SetActive(false);
        loseUIObject.SetActive(false);
        lifeObject.SetActive(true);
        reviveCoinText.text = gameManagerSO.reviveCoin.ToString();
        UpdateCoinUI();
        gameManagerSO.isGameStart = false;
    }

    public void DisableReviveUIMethod()
    {
        lifeObject.SetActive(false);
        revivePanel.SetActive(false);
    }

    #endregion
}
