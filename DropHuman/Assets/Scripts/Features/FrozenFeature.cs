using System.Collections;
using TMPro;
using UnityEngine;

public class FrozenFeature : MonoBehaviour
{
    public GameManagerSO gameManagerSO;
    public GameManager gameManager;
    public UIManager uiManager;
    public TextMeshProUGUI freezeCountText;
    public TextMeshProUGUI freezeTimeText;
   


    private void Start()
    {
        freezeTimeText.text=gameManagerSO.frozenTime.ToString();
        gameManagerSO.isFrozenUsed = false;
    }
    private void Update()
    {
        freezeCountText.text = gameManagerSO.freezeCount.ToString();

        freezeCountText.color=gameManagerSO.freezeCount==0? Color.red : Color.white;

    }
    public void FrezeTime()
    {
        if(gameManagerSO.freezeCount > 0&&!gameManagerSO.isGameOver&&!gameManagerSO.isGameWin)
        {
            StartCoroutine(FreezeTimerCoroutine());
            gameManagerSO.freezeCount--;
            gameManager.SaveFeatures();
        }
    }
    private IEnumerator FreezeTimerCoroutine()
    {
        gameManagerSO.isFrozenUsed = true;
        uiManager.VibrateOnce();

        yield return new WaitForSeconds(gameManagerSO.frozenTime);

        uiManager.VibrateOnce();
        gameManagerSO.isFrozenUsed = false;
    }

}
