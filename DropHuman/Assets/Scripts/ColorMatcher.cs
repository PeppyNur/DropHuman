using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class ColorMatcher : MonoBehaviour
{
    #region VARIABLES

    public Block blockCS;
    public GameObject fallingObj;
    public List<GameObject> spawnPoints;
    public List<GameObject> objects = new List<GameObject>();
    public TMP_Text blockCounterText;
    private GameManager gameManager;
    [SerializeField] private Renderer rendererOfChild;

    [Header("ANIMATIONS")]
    public Animator animator;
    public string overAnimationName;
    private bool isAnimStart;

    #endregion

    #region UNITY METHODS

    private void Awake()
    {
        InitializeObjects();

    }
    private void Start()
    {
        GameObject managerObject = GameObject.Find("GameManager");
        if (managerObject != null)
        {
            gameManager = managerObject.GetComponent<GameManager>();
        }
        else
        {
            gameManager = null;
        }

        BlockText();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FallingObject"))
        {
            int indexToRemove = objects.IndexOf(other.gameObject);

            if(indexToRemove != -1)
            {
                Destroy(other.gameObject);
                objects.RemoveAt(indexToRemove);
                BlockText();
            }
           

            if (objects.Count == 0)
            {
                isAnimStart = true;
                animator.SetBool("isAnimStart", isAnimStart);
            }
        }
    }


    #endregion

    #region OTHER METHODS

    void InitializeObjects()
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null) Destroy(obj);
        }

        objects.Clear();

        for (int i = 0; i < spawnPoints.Count; i++)
        {
            GameObject spawnPointGO = spawnPoints[i];
            Transform spawnPoint = spawnPointGO.transform;

            GameObject newObj = Instantiate(fallingObj, spawnPoint.position, spawnPoint.rotation);
            newObj.transform.localScale = new Vector3(1, 1, 1);

            Renderer newRenderer = newObj.GetComponent<Renderer>();
            newRenderer.material = blockCS.colorOfObject;
            objects.Add(newObj);
        }
    }

    void BlockText()
    {
        blockCounterText.text = objects.Count.ToString();
    }

    void UpdateAfterAnimation()
    {
        Debug.Log("Animasyon bitti, obje yok ediliyor...");

        isAnimStart = false;

        if (gameManager != null && gameManager.blocks.Contains(gameObject))
            gameManager.blocks.Remove(gameObject);

        Destroy(gameObject);
    }

    #endregion
}
