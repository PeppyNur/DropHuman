using UnityEngine;

public class BombFeature : MonoBehaviour
{
    public bool Activate(GameObject target)
    {
        if (target.CompareTag("Block"))
        {
            Debug.Log("Bomba ile yok edildi: " + target.name);

            Destroy(target);

            return true;
        }

        Debug.Log("Bomba i�in ge�ersiz hedef: " + target.name);
        return false;
    }

}
