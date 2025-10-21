using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MagnetFeature : MonoBehaviour
{
    public GameManagerSO gameManagerSO;
    public GameManager gameManager;
    public TextMeshProUGUI magnetCountText;
    public float raiseY;

    [SerializeField] private GameObject touchedBlock;
    private string tagOfTouchedObject;

    private bool areBlocksRaised = false; 
    private float[] originalYPositions;   

    private void Start()
    {
        UpdateMagnetText();

        originalYPositions = new float[gameManager.blocks.Count];
        for (int i = 0; i < gameManager.blocks.Count; i++)
        {
            originalYPositions[i] = gameManager.blocks[i].transform.position.y;
        }
    }

    private void Update()
    {
        //magnet kullanýmý bittiðinde bloklarý eski yerine geri getirir
        if (!gameManagerSO.isMagnetUsed && areBlocksRaised && gameManagerSO.magnetCount>0)
        {
            ResetBlocksPosition();
        }

        if (!gameManagerSO.isMagnetUsed) return;

        //hangi objeye dokunulduðuna göre iþlem yapar
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    touchedBlock = hit.collider.gameObject;
                    tagOfTouchedObject = hit.collider.tag;

                    if (gameManagerSO.magnetCount > 0 && tagOfTouchedObject=="Block")
                    {
                        MagnetObjectsFromList();
                        gameManagerSO.magnetCount--;
                        UpdateMagnetText();
                    }

                    gameManagerSO.isMagnetUsed = false;
                }
            }
        }
    }

    //magnet butonunda kullanýlan method
    public void MagnetButtonMethod()
    {
        if (!areBlocksRaised)
        {
            RaiseBlocks();
        }

        gameManagerSO.isMagnetUsed = true;
    }

    //Bloklarý yerden yükseltir
    void RaiseBlocks()
    {
        for (int i = 0; i < gameManager.blocks.Count; i++)
        {
            Vector3 pos = gameManager.blocks[i].transform.position;
            pos.y += raiseY;
            gameManager.blocks[i].transform.position = pos;
        }

        areBlocksRaised = true;
    }

    //Bloklarýn yüksekliklerini sýfýrlar
    void ResetBlocksPosition()
    {
        for (int i = 0; i < gameManager.blocks.Count; i++)
        {
            Vector3 pos = gameManager.blocks[i].transform.position;
            pos.y = originalYPositions[i];
            gameManager.blocks[i].transform.position = pos;
        }

        areBlocksRaised = false;
    }

    //Magnetin özelliði burda (ayný renkli objeleri siler)
    void MagnetObjectsFromList()
    {
        if (touchedBlock != null && tagOfTouchedObject == "Block")
        {
            ColorMatcher colorMatcher = touchedBlock.GetComponent<ColorMatcher>();
            for (int i = colorMatcher.objects.Count - 1; i >= 0; i--)
            {
                GameObject obj = colorMatcher.objects[i];
                if (obj != null)
                {
                    Destroy(obj);
                }
                colorMatcher.objects.RemoveAt(i);
            }
            colorMatcher.BlockText();
            colorMatcher.DestroyBlock();
        }
    }

    //Magnet Textini günceller
    void UpdateMagnetText()
    {
        magnetCountText.text = gameManagerSO.magnetCount.ToString();
        magnetCountText.color = gameManagerSO.magnetCount == 0 ? Color.red : Color.white;
    }
}
