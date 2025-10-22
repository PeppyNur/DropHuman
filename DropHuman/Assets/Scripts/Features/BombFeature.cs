using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BombFeature : MonoBehaviour
{
    public GameManagerSO gameManagerSO;
    public GameManager gameManager;
    public TextMeshProUGUI bombCountText;
    public float raiseY;

    private bool areSpheresRaised = false;
    private List<GameObject> fallingObjects = new List<GameObject>();
    private float[] originalYPositions;

    private void Update()
    {
        UpdateBombText();

        if (!gameManagerSO.isBombUsed && areSpheresRaised)
        {
            ResetSpherePosition();
        }

        if (!gameManagerSO.isBombUsed) return;

        // Dokunma kontrolü
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject touchedObject = hit.collider.gameObject;
                    string tagOfTouchedObject = hit.collider.tag;

                    if (gameManagerSO.bombCount > 0 && tagOfTouchedObject == "FallingObject")
                    {
                        BombSphere(touchedObject);
                        gameManagerSO.bombCount--;
                        UpdateBombText();
                    }

                    gameManagerSO.isBombUsed = false;
                }
            }
        }
    }

    // Bomb butonuna basýldýðýnda çaðrýlýr
    public void BombButtonMethod()
    {
        UpdateBombText();

        if (!areSpheresRaised && gameManagerSO.bombCount>0)
        {
            SphereObjectsFromScene(); 
            RaiseSpheres();            
        }

        gameManagerSO.isBombUsed = true;
    }

    // Sahnedeki tüm FallingObject tag'li objeleri listeler
    void SphereObjectsFromScene()
    {
        fallingObjects.Clear();

        GameObject[] objects = GameObject.FindGameObjectsWithTag("FallingObject");
        foreach (GameObject obj in objects)
        {
            fallingObjects.Add(obj);
        }

        originalYPositions = new float[fallingObjects.Count];
        for (int i = 0; i < fallingObjects.Count; i++)
        {
            originalYPositions[i] = fallingObjects[i].transform.position.y;
        }
    }

    // Objeleri yükseltir
    void RaiseSpheres()
    {
        for (int i = 0; i < fallingObjects.Count; i++)
        {
            Vector3 pos = fallingObjects[i].transform.position;
            pos.y += raiseY;
            fallingObjects[i].transform.position = pos;
        }

        areSpheresRaised = true;
    }

    // Objeleri eski y pozisyonlarýna getirir
    void ResetSpherePosition()
    {
        for (int i = 0; i < fallingObjects.Count; i++)
        {
            if (fallingObjects[i] != null)
            {
                Vector3 pos = fallingObjects[i].transform.position;
                pos.y = originalYPositions[i];
                fallingObjects[i].transform.position = pos;
            }
        }

        areSpheresRaised = false;
    }

    // Dokunulan objeyi ilgili ColorMatcher listesinden sil ve sahneden yok et
    void BombSphere(GameObject touchedObject)
    {
        if (touchedObject == null) return;

        ColorMatcher[] allBlocks = Object.FindObjectsByType<ColorMatcher>(FindObjectsSortMode.None);
        foreach (ColorMatcher block in allBlocks)
        {
            if (block.objects.Contains(touchedObject))
            {
                block.objects.Remove(touchedObject); 
                Destroy(touchedObject);               

                block.BlockText();
                block.DestroyBlock();

                break; 
            }
        }
    }


    // Bomb sayýsýný UI'ye güncelle
    void UpdateBombText()
    {
        bombCountText.text = gameManagerSO.bombCount.ToString();
        bombCountText.color = gameManagerSO.bombCount == 0 ? Color.red : Color.white;
    }
}
