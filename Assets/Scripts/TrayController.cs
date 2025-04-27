using UnityEngine;

public class TrayDragHandler : MonoBehaviour
{
    private Vector3 offset;
    private Vector3 targetPosition;
    private bool isDragging = false;
    private Camera cam;

    private Transform[] snapPoints;
    public float snapThreshold = 1.5f;

    [Header("Auto Grid Detection")]
    public GameObject floorObject;
    private float minX, maxX, minZ, maxZ;

    private Transform lastHighlighted;
    public Material defaultMat;
    public Material highlightMat;

    void Start()
    {
        cam = Camera.main;
        targetPosition = transform.position;

        GameObject[] foundPoints = GameObject.FindGameObjectsWithTag("SnapPoint");
        snapPoints = new Transform[foundPoints.Length];
        for (int i = 0; i < foundPoints.Length; i++)
            snapPoints[i] = foundPoints[i].transform;

        if (floorObject != null)
        {
            Renderer rend = floorObject.GetComponent<Renderer>();
            if (rend != null)
            {
                Bounds bounds = rend.bounds;
                minX = bounds.min.x;
                maxX = bounds.max.x;
                minZ = bounds.min.z;
                maxZ = bounds.max.z;
            }
            else
            {
                Debug.LogError("Floor object is missing a Renderer!");
            }
        }
        else
        {
            Debug.LogError("No floor object assigned in TrayDragHandler.");
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            TryStartDrag(Input.mousePosition);
        else if (Input.GetMouseButtonUp(0))
            StopDrag();
        else if (isDragging)
            Drag(Input.mousePosition);
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
                TryStartDrag(touch.position);
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                StopDrag();
            else if (isDragging && touch.phase == TouchPhase.Moved)
                Drag(touch.position);
        }
#endif

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

        // 🟡 Highlight nearest snap point while dragging
        if (isDragging)
        {
            float minDistance = Mathf.Infinity;
            Transform closest = null;

            foreach (Transform snap in snapPoints)
            {
                SnapPoint sp = snap.GetComponent<SnapPoint>();
                if (sp == null || sp.isOccupied) continue;

                float dist = Vector3.Distance(transform.position, snap.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = snap;
                }
            }

            if (closest != lastHighlighted)
            {
                if (lastHighlighted != null)
                    lastHighlighted.GetComponent<Renderer>().material = defaultMat;

                if (closest != null)
                    closest.GetComponent<Renderer>().material = highlightMat;

                lastHighlighted = closest;
            }
        }
    }

    void TryStartDrag(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            isDragging = true;
            offset = transform.position - GetWorldPoint(screenPos);

            // 💥 Free current snap point if snapped
            foreach (Transform snap in snapPoints)
            {
                if (Vector3.Distance(transform.position, snap.position) < 0.01f)
                {
                    SnapPoint sp = snap.GetComponent<SnapPoint>();
                    if (sp != null) sp.isOccupied = false;
                }
            }
        }
    }

    void StopDrag()
    {
        if (!isDragging) return;
        isDragging = false;
        SnapToClosestPoint();
    }

    void Drag(Vector2 screenPos)
    {
        Vector3 worldTouch = GetWorldPoint(screenPos);

        float clampedX = Mathf.Clamp(worldTouch.x + offset.x, minX, maxX);
        float clampedZ = Mathf.Clamp(worldTouch.z + offset.z, minZ, maxZ);

        targetPosition = new Vector3(clampedX, transform.position.y, clampedZ);
    }

    Vector3 GetWorldPoint(Vector2 screenPos)
    {
        Vector3 sp = new Vector3(screenPos.x, screenPos.y, cam.WorldToScreenPoint(transform.position).z);
        return cam.ScreenToWorldPoint(sp);
    }

    void SnapToClosestPoint()
    {
        if (snapPoints == null || snapPoints.Length == 0) return;

        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (Transform snap in snapPoints)
        {
            SnapPoint sp = snap.GetComponent<SnapPoint>();
            if (sp == null || sp.isOccupied) continue;

            float dist = Vector3.Distance(transform.position, snap.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = snap;
            }
        }

        if (closest != null && minDistance <= snapThreshold)
        {
            SnapPoint sp = closest.GetComponent<SnapPoint>();
            sp.isOccupied = true;
            targetPosition = closest.position;
        }

        //  Clear highlight after drop
        if (lastHighlighted != null)
        {
            lastHighlighted.GetComponent<Renderer>().material = defaultMat;
            lastHighlighted = null;
        }
    }
}
