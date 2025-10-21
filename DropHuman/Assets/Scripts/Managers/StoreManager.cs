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
        // Her bundle'�n butonuna t�klama eventini ekle
        for (int i = 0; i < bundles.Length; i++)
        {
            int index = i; // closure i�in
            bundles[i].buyButton.onClick.AddListener(() => BuyBundle(index));
            UpdateBundleUI(bundles[i]);
        }
    }

    // Butona t�kland���nda �a�r�l�r
    private void BuyBundle(int index)
    {
        Bundle selectedBundle = bundles[index];

        // Coin kontrol�
        if (gameManagerSO.currentCoin >= selectedBundle.spendCoin)
        {
            // Coini d��
            gameManagerSO.currentCoin -= selectedBundle.spendCoin;

            // Oyuncuya item ekle
            gameManagerSO.currentLife += selectedBundle.buyLifeCount;
            gameManagerSO.bombCount += selectedBundle.buyBombCount;
            gameManagerSO.magnetCount += selectedBundle.buyMagnetCount;
            gameManagerSO.freezeCount += selectedBundle.buyFrozeCount;

            uiManager.UpdateCoinUI();
            uiManager.UpdateLifeUI(gameManagerSO.currentLife, gameManagerSO.maxLife);
            // UI'lar� g�ncelle
            UpdateBundleUI(selectedBundle);
        }
        else
        {
            Debug.Log("Yeterli coin yok!");
        }
    }

    // Bundle UI'lar�n� g�ncelle
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



