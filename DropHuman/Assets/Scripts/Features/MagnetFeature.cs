using System.Collections.Generic;
using UnityEngine;

public class MagnetFeature : MonoBehaviour
{
    // Hedeflenen obje ve o objeyle ayný türdeki tüm bloklarý yok etmeyi dener
    public bool Activate(GameObject target, List<GameObject> allBlocks)
    {
        Block targetBlockScript = target.GetComponent<Block>();

        if (targetBlockScript == null || !target.CompareTag("Block"))
        {
            Debug.Log("Mýknatýs için geçersiz hedef: " + target.name);
            return false;
        }

        var targetType = targetBlockScript.colorOfObject;

        Debug.Log("Mýknatýs '" + targetType + "' türündeki bloklarý yok edecek.");

        List<GameObject> blocksToDestroy = new List<GameObject>();

        foreach (GameObject blockGO in allBlocks)
        {
            if (blockGO == null) continue;

            Block blockScript = blockGO.GetComponent<Block>();

            if (blockScript != null && blockScript.colorOfObject == targetType)
            {
                blocksToDestroy.Add(blockGO);
            }
        }

        foreach (GameObject blockToDestroy in blocksToDestroy)
        {
            Destroy(blockToDestroy);
        }

        return true;
    }
}
