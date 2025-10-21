using TMPro;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class Bundle
{
    public string name;
    public int spendCoin;
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
    public Bundle[] bundles;
    public UIManager uiManager;
    private void Start()
    {
        // Her bundle'ýn butonuna týklama eventini ekle
        for (int i = 0; i < bundles.Length; i++)
        {
            int index = i; // closure için
            bundles[i].buyButton.onClick.AddListener(() => BuyBundle(index));
            UpdateBundleUI(bundles[i]);
        }
    }

    // Butona týklandýðýnda çaðrýlýr
    private void BuyBundle(int index)
    {
        Bundle selectedBundle = bundles[index];

        // Coin kontrolü
        if (gameManagerSO.currentCoin >= selectedBundle.spendCoin)
        {
            // Coini düþ
            gameManagerSO.currentCoin -= selectedBundle.spendCoin;

            // Oyuncuya item ekle
            gameManagerSO.currentLife += selectedBundle.buyLifeCount;
            gameManagerSO.bombCount += selectedBundle.buyBombCount;
            gameManagerSO.magnetCount += selectedBundle.buyMagnetCount;
            gameManagerSO.freezeCount += selectedBundle.buyFrozeCount;

            uiManager.UpdateCoinUI();
            uiManager.UpdateLifeUI(gameManagerSO.currentLife, gameManagerSO.maxLife);
            // UI'larý güncelle
            UpdateBundleUI(selectedBundle);
        }
        else
        {
            Debug.Log("Yeterli coin yok!");
        }
    }

    // Bundle UI'larýný güncelle
    private void UpdateBundleUI(Bundle bundle)
    {
        if (bundle.priceText != null)
            bundle.priceText.text = bundle.spendCoin.ToString();

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



