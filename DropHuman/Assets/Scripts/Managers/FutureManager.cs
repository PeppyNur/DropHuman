using UnityEngine;

public class FutureManager : MonoBehaviour
{
    public GameObject touchedObject;
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    touchedObject = hit.collider.gameObject;
                    Debug.Log(touchedObject);
                }
                else
                {
                    Debug.Log("Hiçbir objeye dokunulmadý.");
                }
            }
        }
    }
}
