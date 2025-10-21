using System.Collections.Generic;
using UnityEngine;

public class MagnetFeature : MonoBehaviour
{
    // Hedeflenen obje ve o objeyle ayn� t�rdeki t�m bloklar� yok etmeyi dener
    public bool Activate(GameObject target, List<GameObject> allBlocks)
    {
        Block targetBlockScript = target.GetComponent<Block>();

        if (targetBlockScript == null || !target.CompareTag("Block"))
        {
            Debug.Log("M�knat�s i�in ge�ersiz hedef: " + target.name);
            return false;
        }

        var targetType = targetBlockScript.colorOfObject;

        Debug.Log("M�knat�s '" + targetType + "' t�r�ndeki bloklar� yok edecek.");

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
