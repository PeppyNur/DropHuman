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
    [SerializeField] private UIManager uiManager;
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
        GameObject uiObject = GameObject.Find("UIManager");
        if (uiObject != null)
        {
            uiManager = uiObject.GetComponent<UIManager>();
        }
        else
        {
            uiManager = null;
        }
        BlockText();
    }
    private void Update()
    {
                objects.RemoveAll(x => x == null);

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("FallingObject"))
        {
            int indexToRemove = objects.IndexOf(other.gameObject);

            if(indexToRemove != -1)
            {
                Destroy(other.gameObject);
                uiManager.VibrateOnce();
                uiManager.PlayClickSounds();
                objects.RemoveAt(indexToRemove);
                BlockText();
                
            }
           

            DestroyBlock();
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

            GameObject newObj = Instantiate(fallingObj, spawnPoint.position, spawnPoint.rotation, spawnPoint);

            Renderer newRenderer = newObj.GetComponent<Renderer>();
            newRenderer.material = blockCS.colorOfObject;
            objects.Add(newObj);
        }
    }

    public void BlockText()
    {
        blockCounterText.text = objects.Count.ToString();
    }

    public void DestroyBlock()
    {
        if (objects.Count == 0)
        {
            isAnimStart = true;
            animator.SetBool("isAnimStart", isAnimStart);
        }
    }
    void UpdateAfterAnimation()
    {
        isAnimStart = false;

        if (gameManager != null && gameManager.blocks.Contains(gameObject))
            gameManager.blocks.Remove(gameObject);

        Destroy(gameObject);
    }

    #endregion
}
