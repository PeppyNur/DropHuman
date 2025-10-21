using System.Collections;
using TMPro;
using UnityEngine;

public class FrozenFeature : MonoBehaviour
{
    public GameManagerSO gameManagerSO;
    public TextMeshProUGUI freezeCountText;
    public TextMeshProUGUI freezeTimeText;
    public int freezeGiftLevel;
    private bool isGiftAdded;

    private void Start()
    {
        freezeTimeText.text=gameManagerSO.frozenTime.ToString();
        gameManagerSO.isFrozenUsed = false;
    }
    private void Update()
    {
        freezeCountText.text = gameManagerSO.freezeCount.ToString();

        freezeCountText.color=gameManagerSO.freezeCount==0? Color.red : Color.white;

        if (gameManagerSO.currentLevel == freezeGiftLevel && !isGiftAdded)
        {
            gameManagerSO.freezeCount++;
            isGiftAdded = true;
        }
        
    }
    public void FrezeTime()
    {
        if(gameManagerSO.freezeCount > 0&&!gameManagerSO.isGameOver&&!gameManagerSO.isGameWin)
        {
            StartCoroutine(FreezeTimerCoroutine());
            gameManagerSO.freezeCount--;
        }
    }
    private IEnumerator FreezeTimerCoroutine()
    {
        gameManagerSO.isFrozenUsed = true;

        yield return new WaitForSeconds(gameManagerSO.frozenTime);

        gameManagerSO.isFrozenUsed = false;
    }
}
