using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class MapManager : MonoBehaviour
{
    #region VARIABLES

    public GameManagerSO gameManagerSO;
    public GameManager gameManager;

    private GameObject currentMapInstance;

    #endregion

    #region MAP GENERATOR

    // Kaydedilmiş seviyeyi yükler, eğer mevcutsa aynı prefab'ı tekrar oluşturur
    public void LoadSavedLevel()
    {
        gameManagerSO.isGameWin = false;
        gameManagerSO.isGameOver = false;

        if (gameManagerSO.savedLevel > 0 && gameManagerSO.currentLife > 0)
        {
            gameManagerSO.currentLevel = gameManagerSO.savedLevel;

            if (gameManagerSO.currentMap != null)
            {
                ClearCurrentMap();

                currentMapInstance = Instantiate(gameManagerSO.currentMap, Vector3.zero, Quaternion.identity);
                gameManagerSO.SetTimerByLevelType();
                gameManagerSO.isGameStart = true;
            }
            else
            {
                Debug.LogWarning("Kayıtlı currentMap bulunamadı, yeni map yüklenecek.");
                LoadNextLevel();
                gameManagerSO.isGameStart = true;

            }
        }
    }

    // Yeni bir seviye yüklendiğinde uygun haritayı seçip oluşturur
    public void LoadNextLevel()
    {
        gameManagerSO.isGameWin = false;
        gameManagerSO.isGameOver = false;

        if (gameManagerSO.currentMap == null)
        {
            if (gameManagerSO.currentLife > 0)
            {
                ClearCurrentMap();

                GameObject nextMapPrefab = SelectNextMap();

                if (nextMapPrefab != null)
                {
                    currentMapInstance = Instantiate(nextMapPrefab, Vector3.zero, Quaternion.identity);

                    // Mevcut map ve level bilgilerini kaydeder
                    gameManagerSO.currentMap = nextMapPrefab;
                    gameManagerSO.savedLevel = gameManagerSO.currentLevel;

                    gameManagerSO.SetTimerByLevelType();
                    gameManagerSO.isGameStart = true;
                }
                else
                {
                    Debug.LogWarning("Map prefab bulunamadı!");
                }
            }
        }
        else
        {
            LoadSavedLevel();
        }
    }

    // Sıradaki map prefab'ını seviyeye göre belirler
    private GameObject SelectNextMap()
    {
        if (gameManagerSO.currentLevel < gameManagerSO.trainingLevelMaps.Count)
        {
            gameManagerSO.currentLevelType = LevelType.Training;
            return gameManagerSO.trainingLevelMaps[gameManagerSO.currentLevel];
        }

        if (gameManagerSO.currentLevel % 10 == 1 && gameManagerSO.currentLevel > gameManagerSO.trainingLevelMaps.Count)
        {
            if (gameManagerSO.hardLevelMaps.Count > 0)
            {
                gameManagerSO.currentLevelType = LevelType.Hard;
                int randomIndex = Random.Range(0, gameManagerSO.hardLevelMaps.Count);
                return gameManagerSO.hardLevelMaps[randomIndex];
            }
        }

        if (gameManagerSO.currentLevel >= gameManagerSO.trainingLevelMaps.Count)
        {
            if (gameManagerSO.normalLevelMaps.Count > 0)
            {
                gameManagerSO.currentLevelType = LevelType.Normal;
                int randomIndex = Random.Range(0, gameManagerSO.normalLevelMaps.Count);
                return gameManagerSO.normalLevelMaps[randomIndex];
            }
        }

        return null;
    }

    // Mevcut haritayı ve sahnedeki düşen objeleri temizler
    public void ClearCurrentMap()
    {
        if (currentMapInstance != null)
        {
            Destroy(currentMapInstance);
            currentMapInstance = null;
        }

        GameObject[] fallingObjects = GameObject.FindGameObjectsWithTag("FallingObject");
        foreach (GameObject obj in fallingObjects)
        {
            Destroy(obj);
        }

        gameManagerSO.isGameStart = false;
    }

    #endregion
}
