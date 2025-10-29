using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Bundle
{
    public string name;
    public int priceOfBundle;
    public int buyLifeCount;
    public int buyBombCount;
    public int buyMagnetCount;
    public int buyFrozeCount;

    public Button buyButton;

    public TextMeshProUGUI priceText;
    public TextMeshProUGUI lifeCountText;
    public TextMeshProUGUI bombCountText;
    public TextMeshProUGUI magnetCountText;
    public TextMeshProUGUI frozeCountText;
}

public class StoreManager : MonoBehaviour
{
    public GameManagerSO gameManagerSO;
    public GameManager gameManager;
    public Bundle[] bundles;
    public UIManager uiManager;
    private void Start()
    {
        for (int i = 0; i < bundles.Length; i++)
        {
            int index = i; 
            bundles[i].buyButton.onClick.AddListener(() => BuyBundle(index));
            UpdateBundleUI(bundles[i]);
        }
    }

    private void BuyBundle(int index)
    {
        Bundle selectedBundle = bundles[index];

        if (gameManagerSO.currentCoin >= selectedBundle.priceOfBundle)
        {
            gameManagerSO.currentCoin -= selectedBundle.priceOfBundle;

            gameManagerSO.currentLife += selectedBundle.buyLifeCount;
            gameManagerSO.bombCount += selectedBundle.buyBombCount;
            gameManagerSO.magnetCount += selectedBundle.buyMagnetCount;
            gameManagerSO.freezeCount += selectedBundle.buyFrozeCount;

            gameManager.SaveCoin();
            gameManager.SaveLife();
            gameManager.SaveFeatures();
            uiManager.UpdateCoinUI();
            uiManager.UpdateLifeUI(gameManagerSO.currentLife, gameManagerSO.maxLife);
            uiManager.PlayClickSounds();
            UpdateBundleUI(selectedBundle);
        }
        else
        {
            Debug.Log("Yeterli coin yok!");
        }
    }

    private void UpdateBundleUI(Bundle bundle)
    {
        if (bundle.priceText != null)
            bundle.priceText.text = bundle.priceOfBundle.ToString();

        if (bundle.lifeCountText != null)
            bundle.lifeCountText.text = bundle.buyLifeCount.ToString();

        if (bundle.bombCountText != null)
            bundle.bombCountText.text = bundle.buyBombCount.ToString();

        if (bundle.magnetCountText != null)
            bundle.magnetCountText.text = bundle.buyMagnetCount.ToString();

        if (bundle.frozeCountText != null)
            bundle.frozeCountText.text = bundle.buyFrozeCount.ToString();
    }

}



