using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    public GameManagerSO gameManagerSO;
    public GameManager gameManager;
    public UIManager uiManager;
    private float timer;

    //s�n�rs�z can modu 
    public bool unlimitedLifeActive = false;
    public float unlimitedLifeTimer = 0f;
    public int lifeAtStartUnlimitedMode = 0;
    public float unlimitedModeElapsedTime = 0f;

    private void Start()
    {
        RecalculateFromOffline();

    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            gameManager.SaveExitTime();
        }
        else
        {
            RecalculateFromOffline();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            gameManager.SaveExitTime();
        }
        else
        {
            RecalculateFromOffline();
        }
    }

    private void Update()
    {
        if(unlimitedLifeActive)
        {
            unlimitedLifeTimer += Time.deltaTime;
            unlimitedModeElapsedTime += Time.deltaTime;

            gameManagerSO.currentLife = lifeAtStartUnlimitedMode;
            uiManager.UpdateLifeUI(gameManagerSO.currentLife, gameManagerSO.maxLife);
            uiManager.UpdateLifeTimerUI(unlimitedLifeTimer);

            if (unlimitedLifeTimer < 0)
            {
                unlimitedLifeActive = false;
                unlimitedLifeTimer = 0;

                int regenCount = Mathf.FloorToInt(unlimitedModeElapsedTime/(gameManagerSO.lifeCountdown * 60));
                
                if(regenCount > 0)
                    gameManagerSO.currentLife = Mathf.Min(gameManagerSO.currentLife+regenCount, gameManagerSO.maxLife);

                double remainder =unlimitedModeElapsedTime % (gameManagerSO.lifeCountdown*60f);
                timer = Mathf.Max((float)(gameManagerSO.lifeCountdown*60-remainder), 0);
                unlimitedModeElapsedTime = 0f;
            }
        }

        else
        {
            if(gameManagerSO.currentLife<gameManagerSO.maxLife)
            {
                timer -= Time.deltaTime;
                if(timer <= 0)
                {
                    RegenerateLife();
                    timer = gameManagerSO.lifeCountdown * 60f;
                }
                uiManager.UpdateLifeTimerUI(timer);
            }
            else
            {
                uiManager.SetFullLifeText();

            }
        }
    }

    private void RecalculateFromOffline()
    {
        float offlineSeconds = gameManager.GetOfflineTimePassed();
        int secondsPerLife = Mathf.RoundToInt(gameManagerSO.lifeCountdown * 60f);

        if (offlineSeconds > 0)
        {
            int regenCount = Mathf.FloorToInt(offlineSeconds / secondsPerLife);
            if (regenCount > 0)
            {
                gameManagerSO.currentLife = Mathf.Min(gameManagerSO.currentLife + regenCount, gameManagerSO.maxLife);
            }

            double remainder = offlineSeconds % secondsPerLife;
            timer = Mathf.Max(secondsPerLife - (float)remainder, 0f);
        }
        else
        {
            // İlk açılış veya kayıt yoksa tam süre ile başla
            timer = secondsPerLife;
        }

        uiManager.UpdateLifeUI(gameManagerSO.currentLife, gameManagerSO.maxLife);
        if (gameManagerSO.currentLife < gameManagerSO.maxLife)
        {
            uiManager.UpdateLifeTimerUI(timer);
        }
        else
        {
            uiManager.SetFullLifeText();
        }
    }
    void RegenerateLife()
    {
        if (gameManagerSO.currentLife < gameManagerSO.maxLife)
        {
            gameManagerSO.currentLife += 1;
            uiManager.UpdateLifeUI(gameManagerSO.currentLife, gameManagerSO.maxLife);
        }
    }

    public void ActivateUnlimitedLife(float hours)
    {
        unlimitedLifeActive = true;
        unlimitedLifeTimer = hours * 3600f;
        lifeAtStartUnlimitedMode = gameManagerSO.currentLife;
        unlimitedModeElapsedTime = 0f;
        
    }
}
