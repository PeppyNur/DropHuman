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
    private void Start()
    {
        if (gameManagerSO.currentLevel == 0)
        {
            gameManagerSO.currentMap=null;
            gameManager.SaveMap();
        }
    }
    #region MAP GENERATOR

    // Kaydedilmiş seviyeyi yükler, eğer mevcutsa aynı prefab'ı tekrar oluşturur
    public void LoadSavedLevel()
    {
        gameManagerSO.isGameWin = false;
        gameManagerSO.isGameOver = false;

        if (gameManagerSO.savedLevel > 0 && gameManagerSO.currentLife > 0)
        {
            gameManagerSO.currentLevel = gameManagerSO.savedLevel;
            gameManager.SaveLevel();

            if (gameManagerSO.currentMap != null)
            {
                ClearCurrentMap();

                currentMapInstance = Instantiate(gameManagerSO.currentMap, Vector3.zero, Quaternion.identity);
                gameManagerSO.SetTimerByLevelType();
                gameManagerSO.isGameStart = true;
            }
            else
            {
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
                    gameManager.SaveLevel();
                    gameManager.SaveMap();

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
               return GetUniqueNextMap(gameManagerSO.hardLevelMaps);
            }
        }

        if (gameManagerSO.currentLevel >= gameManagerSO.trainingLevelMaps.Count)
        {
            if (gameManagerSO.normalLevelMaps.Count > 0)
            {
                gameManagerSO.currentLevelType = LevelType.Normal;
                return GetUniqueNextMap(gameManagerSO.normalLevelMaps);

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

    private GameObject GetUniqueNextMap(List<GameObject> mapList)
    {
        List<GameObject> availableMaps = new List<GameObject>(mapList);

        GameObject currentMap = gameManagerSO.currentMap;
        if(currentMap != null && availableMaps.Contains(currentMap))
        {
            availableMaps.Remove(currentMap);
        }

        if(availableMaps.Count > 0)
        {
            int randomIndex = Random.Range(0,availableMaps.Count);
            GameObject newMap = availableMaps[randomIndex];

            gameManagerSO.currentMap = newMap;
            gameManager.SaveMap();

            return newMap;
        }
        else if(currentMap != null)
        {
            return currentMap;
        }
        return null;
    }

    #endregion
}
