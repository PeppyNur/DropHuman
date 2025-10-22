using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Block : MonoBehaviour
{
    #region VARİABLES

    [Header("Grid ve Pozisyon Ayarları")]
    [SerializeField] float gridSize = 1f;
    [SerializeField] float fixedY = 0.25f;

    [Header("Çarpışma Kontrol Ayarları")]
    [SerializeField] float overlapTolerance = 0.001f;

    private Collider[] selfColliders;
    [SerializeField] LayerMask checkLayer;

    [Header("Child Object Settings")]
    [SerializeField] List<GameObject> childObjects;
    public ColorMaterialsSO materialSO;
    public GameObject canvasObject;
    public Material colorOfObject;

    // --- DİĞER DEĞİŞKENLER ---
    Vector3 initialPosition;
    Vector3 lastValidPosition;
    bool dragging = false;
    bool isCurrentPositionClear = true;
    private Vector3 bestSnapPosition;
    private Vector3 dragOffset;

    // --- GİZMO İÇİN EK DEĞİŞKENLER ---
    private List<(Vector3 center, Vector3 halfSize, Quaternion rotation, bool clear)> gizmoOverlapChecks = new List<(Vector3, Vector3, Quaternion, bool)>();
    private List<RaycastHit> gizmoBoxCastHits = new List<RaycastHit>();
    private Vector3 gizmoSweepStart;
    private Vector3 gizmoSweepDirection;
    private float gizmoSweepDistance;

    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        selfColliders = GetComponentsInChildren<Collider>();
        ChangeChildObjectsColor();
    }

    private void Start()
    {
        initialPosition = transform.position;
        lastValidPosition = transform.position;
        AttachCanvasToBottomRightAnchor();

        lastValidPosition = transform.position;
    }

    private void Update()
    {
        HandleTouchInputWithRaycast();

        if (dragging)
        {
            isCurrentPositionClear = CheckPlacementOverlap(transform.position, false);
        }
    }
    #endregion
    #region PLACEMENT
    public bool CheckPlacementOverlap(Vector3 targetPos, bool isSweepTest)
    {
        if (selfColliders == null || selfColliders.Length == 0) return true;

        bool foundObstacle = false;

        if (selfColliders[0] == selfColliders.FirstOrDefault(c => c.transform.position == transform.position))
        {
            gizmoOverlapChecks.Clear();
            gizmoBoxCastHits.Clear();
        }

        Quaternion checkRotation = transform.rotation;

        foreach (var col in selfColliders)
        {
            BoxCollider boxCol = col as BoxCollider;
            if (boxCol == null)
            {
                Vector3 fallbackOffset = col.bounds.center - transform.position;
                Vector3 checkCenterFallback = targetPos + fallbackOffset;

                if (!isSweepTest)
                {
                    gizmoOverlapChecks.Add((checkCenterFallback, col.bounds.extents, col.transform.rotation, false));
                }
                continue;
            }

            Vector3 worldCenter = col.bounds.center;

            Vector3 globalOffsetFromParent = worldCenter - transform.position;

            Vector3 checkCenter = targetPos + globalOffsetFromParent;


            Vector3 localSize = boxCol.size;
            Vector3 lossyScale = boxCol.transform.lossyScale;
            Vector3 scaledSize = Vector3.Scale(localSize, lossyScale);
            Vector3 baseHalfSize = scaledSize / 2f;

            Vector3 usedHalfSize;
            if (!isSweepTest)
            {
                Vector3 tolerantHalfSize = new Vector3(
                    baseHalfSize.x - overlapTolerance / 2f,
                    baseHalfSize.y,
                    baseHalfSize.z - overlapTolerance / 2f
                );
                tolerantHalfSize.x = Mathf.Max(0.001f, tolerantHalfSize.x);
                tolerantHalfSize.z = Mathf.Max(0.001f, tolerantHalfSize.z);

                usedHalfSize = tolerantHalfSize;
            }
            else
            {
                usedHalfSize = baseHalfSize;
            }

            if (isSweepTest)
            {
                Vector3 sweepDirection = targetPos - lastValidPosition;
                float sweepDistance = sweepDirection.magnitude;

                if (col == selfColliders[0])
                {
                    gizmoSweepStart = lastValidPosition;
                    gizmoSweepDirection = sweepDirection.normalized;
                    gizmoSweepDistance = sweepDistance;
                }

                if (sweepDistance > 0.001f)
                {
                    sweepDirection.Normalize();
                    RaycastHit hit;

                    float skinWidth = 0.001f;
                    Vector3 boxCastStart = lastValidPosition + globalOffsetFromParent + sweepDirection * skinWidth;

                    Vector3 boxCastHalfSize = new Vector3(
                         usedHalfSize.x - skinWidth,
                         usedHalfSize.y,
                         usedHalfSize.z - skinWidth
                    );

                    if (Physics.BoxCast(
                        boxCastStart,
                        boxCastHalfSize,
                        sweepDirection,
                        out hit,
                        checkRotation,
                        sweepDistance,
                        checkLayer
                    ))
                    {
                        if (!selfColliders.Contains(hit.collider))
                        {
                            foundObstacle = true;
                            gizmoBoxCastHits.Add(hit);
                        }
                    }
                }
            }
            else
            {
                Collider[] hitColliders = Physics.OverlapBox(checkCenter, usedHalfSize, checkRotation, checkLayer);

                bool clear = true;

                foreach (var hitCollider in hitColliders)
                {
                    if (!selfColliders.Contains(hitCollider))
                    {
                        foundObstacle = true;
                        clear = false;
                        break;
                    }
                }
                gizmoOverlapChecks.Add((checkCenter, usedHalfSize, checkRotation, clear));
            }

            if (foundObstacle) break;
        }

        isCurrentPositionClear = !foundObstacle;
        return !foundObstacle;
    }

    #endregion

    #region TOUCHINPUT
    private void HandleTouchInputWithRaycast()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float touchRayDistance = 1000f;
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (touch.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hit, touchRayDistance))
                {
                    if (hit.transform == this.transform || hit.transform.IsChildOf(this.transform))
                    {
                        dragging = true;
                        initialPosition = transform.position;
                        lastValidPosition = transform.position;
                        bestSnapPosition = Vector3.zero;

                        dragOffset = transform.position - hit.point;
                        dragOffset.y = 0;
                    }
                }
            }

            else if (touch.phase == TouchPhase.Moved && dragging)
            {
                if (Physics.Raycast(ray, out hit, touchRayDistance, checkLayer))
                {
                    Vector3 newXZPosition = hit.point + dragOffset;
                    Vector3 attemptedPosition = new Vector3(newXZPosition.x, fixedY, newXZPosition.z);


                    // 1. Hedef Alan Temiz mi? (Overlap Testi)
                    bool targetAreaClear = CheckPlacementOverlap(attemptedPosition, false);

                    if (targetAreaClear)
                    {
                        transform.position = attemptedPosition;
                        lastValidPosition = attemptedPosition;
                    }
                    else
                    {
                        transform.position = lastValidPosition;
                    }
                }
            }

            else if (touch.phase == TouchPhase.Ended && dragging)
            {
                float snappedX = Mathf.Round(transform.position.x / gridSize) * gridSize;
                float snappedZ = Mathf.Round(transform.position.z / gridSize) * gridSize;
                Vector3 targetPosition = new Vector3(snappedX, fixedY, snappedZ);

                // Snap sonrası sınır kontrolü

                transform.position = targetPosition;

                bool isAreaClear = CheckPlacementOverlap(targetPosition, false);

                dragging = false;
            }
        }
    }

    #endregion

    #region CHILD OBJECTS
    // CHİLD OBJELERİN RENKLERİNİ SO'DA BULUNAN RENKLERDEN BİRİ OLACAK ŞEKİLDE ATAR 
    public void ChangeChildObjectsColor()
    {
        Material selectedMaterial = UniqueColorRegistry.GetUniqueMaterial(materialSO);
        if (selectedMaterial == null && materialSO != null && materialSO.blockColors.Count > 0)
        {
            selectedMaterial = materialSO.blockColors[0];
        }

        foreach (GameObject childObject in childObjects)
        {
            if (childObject != null)
            {

                Renderer childRenderer = childObject.GetComponent<Renderer>();

                if (childRenderer != null)
                {
                    childRenderer.material = selectedMaterial;
                }
            }
        }

        colorOfObject = selectedMaterial;
    }

    // CANVA YERİNİ AYARLAMA
    void AttachCanvasToBottomRightAnchor()
    {
        float max_X = float.MinValue;
        float min_Z = float.MaxValue;

        for (int i = 0; i < childObjects.Count; i++)
        {
            GameObject child = childObjects[i];
            if (child == null) continue;

            float currentX = child.transform.position.x;
            float currentZ = child.transform.position.z;

            if (currentX > max_X)
            {
                max_X = currentX;
            }

            if (currentZ < min_Z)
            {
                min_Z = currentZ;
            }
        }

        if (max_X == float.MinValue)
        {
            return;
        }

        List<Transform> minZCandidates = new List<Transform>();

        for (int i = 0; i < childObjects.Count; i++)
        {
            GameObject child = childObjects[i];
            if (child == null) continue;

            float currentZ = child.transform.position.z;

            if (Mathf.Abs(currentZ - min_Z) < 0.001f)
            {
                minZCandidates.Add(child.transform);
            }
        }

        Transform finalAnchor = null;

        if (minZCandidates.Count == 1)
        {
            finalAnchor = minZCandidates[0];
        }
        else if (minZCandidates.Count > 1)
        {
            float smallestXDifference = float.MaxValue;

            foreach (Transform candidate in minZCandidates)
            {
                float currentX = candidate.position.x;
                float xDifference = Mathf.Abs(currentX - max_X);

                if (xDifference < smallestXDifference)
                {
                    smallestXDifference = xDifference;
                    finalAnchor = candidate;
                }
            }
        }

        canvasObject.transform.SetParent(finalAnchor);
        canvasObject.transform.localPosition = new Vector3(0f, canvasObject.transform.localPosition.y, 0f);

        //rotasyon ayarlama
        float anchorWorldYRotation = finalAnchor.rotation.eulerAngles.y;

        float correctedYRotation;
        const float tolerance = 0.01f;

        if (Mathf.Abs(anchorWorldYRotation - 0f) < tolerance ||
            Mathf.Abs(anchorWorldYRotation - 360f) < tolerance ||
            Mathf.Abs(anchorWorldYRotation - 180f) < tolerance)
        {
            correctedYRotation = anchorWorldYRotation;
        }
        else
        {
            correctedYRotation = anchorWorldYRotation + 180f;
        }

        canvasObject.transform.localRotation = Quaternion.Euler(0f, correctedYRotation, 0f);
    }

    #endregion

    #region GIZMO
    void OnDrawGizmos()
    {
        DrawGizmosInternal(false);
    }

    void OnDrawGizmosSelected()
    {
        DrawGizmosInternal(true);
    }

    private void DrawGizmosInternal(bool isSelected)
    {
        if (selfColliders == null)
            selfColliders = GetComponentsInChildren<Collider>();

        // --- OverlapBox Görselleştirmesi (Mevcut Konum Kontrolü) ---
        if (isSelected || dragging)
        {
            foreach (var check in gizmoOverlapChecks)
            {
                Gizmos.color = check.clear ? Color.green : Color.red;

                Gizmos.matrix = Matrix4x4.TRS(check.center, check.rotation, Vector3.one);

                Gizmos.DrawWireCube(Vector3.zero, check.halfSize * 2);
            }
            Gizmos.matrix = Matrix4x4.identity;

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.05f);
        }

        // --- BoxCast (Sweep Testi) Görselleştirmesi ---
        if ((isSelected || dragging) && gizmoSweepDistance > 0.001f)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(gizmoSweepStart, 0.05f);

            foreach (var hit in gizmoBoxCastHits)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(gizmoSweepStart, hit.point);
                Gizmos.DrawSphere(hit.point, 0.05f);
            }

            if (gizmoBoxCastHits.Count == 0)
            {
                Gizmos.color = Color.green;
                Vector3 targetEnd = gizmoSweepStart + gizmoSweepDirection * gizmoSweepDistance;
                Gizmos.DrawLine(gizmoSweepStart, targetEnd);
                Gizmos.DrawSphere(targetEnd, 0.05f);
            }
        }
    }
    #endregion
}