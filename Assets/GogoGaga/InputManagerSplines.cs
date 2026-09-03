using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class SplineConnectorManager : MonoBehaviour
{
    [Header("Spline Settings")]
    [SerializeField] private GameObject splinePrefab; // Must contain a SplineContainer + SplineAnimate or similar visualizer

    [Header("Preview Line Settings")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.05f;

    [Header("Target Settings")]
    [SerializeField] private string connectableTag = "Connectable";

    [Header("Highlight Settings")]
    [SerializeField] private Material selectionHighlightMaterial;
    [SerializeField] private Material hoverHighlightMaterial;

    private Camera mainCamera;
    private GameObject selectedObject;
    private GameObject currentHoveredObject;
    private GameObject previewSplineGO;
    private LineRenderer previewLine;

    private Dictionary<GameObject, GameObject> activeSplines = new(); // object → spline GameObject
    private Dictionary<GameObject, Material> originalMaterials = new();

    private float splineRadius = 0.05f;
    private float extraPadding = 0.01f;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleHover();
        HandleClick();
        HandleCancel();

        if (selectedObject != null)
		{
            //UpdatePreviewSplineToMouse();
            UpdatePreviewLineToMouse();

        }
            

        //container.Spline.SetKnot(1, new BezierKnot(mouseWorldPos));

    }

    void HandleClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clicked = hit.collider.gameObject;

            if (!clicked.CompareTag(connectableTag))
                return;

            if (selectedObject == null)
            {
                selectedObject = clicked;
                ApplySelectionHighlight(selectedObject);
                //CreatePreviewSplineFrom(selectedObject);
                CreatePreviewLineFrom(selectedObject);
            }
            else
            {
                if (clicked == selectedObject)
                {
                    //CancelPreviewSpline();
                    CancelPreviewLine();
                    UnhighlightObject(selectedObject);
                    selectedObject = null;
                    return;
                }

                CreateSplineConnection(selectedObject, clicked);
                UnhighlightObject(selectedObject);
                selectedObject = null;
                //CancelPreviewSpline();
                CancelPreviewLine();
            }
        }
    }

    void HandleCancel()
    {
        if (selectedObject != null && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)))
        {
            UnhighlightObject(selectedObject);
            //CancelPreviewSpline();
            CancelPreviewLine();
            selectedObject = null;
        }
    }

    void HandleHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject hovered = hit.collider.gameObject;

            if (!hovered.CompareTag(connectableTag))
            {
                ClearHoverHighlight();
                return;
            }

            if (hovered == selectedObject || hovered == currentHoveredObject)
                return;

            ClearHoverHighlight();
            currentHoveredObject = hovered;
            ApplyHoverHighlight(currentHoveredObject);
        }
        else
        {
            ClearHoverHighlight();
        }
    }

    void CloneMesh(GameObject newSplineGO)
	{
        // Clone mesh to avoid shared references
        var meshFilter = newSplineGO.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh newMesh = new Mesh();
            newMesh.name = "SplineMesh_Instance";
            meshFilter.sharedMesh = newMesh;
        }
    }

    void CreateSplineConnection(GameObject fromObj, GameObject toObj)
    {
        RemoveConnection(fromObj);
        RemoveConnection(toObj);

        GameObject splineGO = Instantiate(splinePrefab);
        SplineContainer container = splineGO.GetComponent<SplineContainer>();

        // Move line positions slightly closer to the camera
        Vector3 fromPos = OffsetInFrontOfObject(fromObj);
        Vector3 toPos = OffsetInFrontOfObject(toObj); 

        container.Spline.Clear();
        container.Spline.Add(new BezierKnot(fromPos));
        container.Spline.Add(new BezierKnot(toPos));

        activeSplines[fromObj] = splineGO;
        activeSplines[toObj] = splineGO;

        CloneMesh(splineGO);
    }

    #region preview_splines
    void CreatePreviewSplineFrom(GameObject startObj)
    {
        previewSplineGO = Instantiate(splinePrefab);
        SplineContainer container = previewSplineGO.GetComponent<SplineContainer>();

        Vector3 startPos = OffsetInFrontOfObject(startObj);

        container.Spline.Clear();
        container.Spline.Add(new BezierKnot(startPos)); //startObj.transform.position bug
        container.Spline.Add(new BezierKnot(startObj.transform.position)); // placeholder for end point

        CloneMesh(previewSplineGO);
    }

    void UpdatePreviewSplineToMouse()
    {		
		if (previewSplineGO == null || selectedObject == null) return;

        SplineContainer container = previewSplineGO.GetComponent<SplineContainer>();
        if (container == null || container.Spline.Count < 2) return;

        Vector3 start = selectedObject.transform.position;
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Mathf.Abs(mainCamera.transform.position.z - selectedObject.transform.position.z)
        ));

        Vector3 adjusted = OffsetZFrom(mouseWorld, selectedObject);

        // Update both knots (or just the end knot)
        //container.Spline.SetKnot(0, new BezierKnot(start));
        container.Spline.SetKnot(1, new BezierKnot(adjusted));
        

	}
	#endregion

	#region preview_linerender
	void CreatePreviewLineFrom(GameObject startObj)
    {
        previewLine = new GameObject("PreviewLine").AddComponent<LineRenderer>();
        previewLine.material = lineMaterial;
        previewLine.startWidth = previewLine.endWidth = lineWidth;
        previewLine.positionCount = 2;
        previewLine.useWorldSpace = true;

        Vector3 startPos = OffsetInFrontOfObject(startObj);

        previewLine.SetPosition(0, startPos);
        previewLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        previewLine.receiveShadows = false;
    }

    void UpdatePreviewLineToMouse()
    {
        if (previewLine == null) return;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            Mathf.Abs(mainCamera.transform.position.z - selectedObject.transform.position.z)
        ));

        Vector3 adjusted = OffsetZFrom(mouseWorld, selectedObject);
        previewLine.SetPosition(1, adjusted);

    }
	#endregion

	void CancelPreviewSpline()
    {
        if (previewSplineGO != null)
            Destroy(previewSplineGO);
        previewSplineGO = null;
    }

    void CancelPreviewLine()
    {
        if (previewLine != null)
            Destroy(previewLine.gameObject);
        previewLine = null;
    }

    void RemoveConnection(GameObject obj)
    {
        if (activeSplines.TryGetValue(obj, out GameObject splineGO))
        {
            Destroy(splineGO);
            foreach (var key in new List<GameObject>(activeSplines.Keys))
            {
                if (activeSplines[key] == splineGO)
                    activeSplines.Remove(key);
            }
        }
    }

	#region Port_UI
	void ApplySelectionHighlight(GameObject obj) => SetMaterial(obj, selectionHighlightMaterial);
    void ApplyHoverHighlight(GameObject obj) => SetMaterial(obj, hoverHighlightMaterial);

    void ClearHoverHighlight()
    {
        if (currentHoveredObject != null && currentHoveredObject != selectedObject)
            UnhighlightObject(currentHoveredObject);
        currentHoveredObject = null;
    }

    void SetMaterial(GameObject obj, Material newMat)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null && newMat != null)
        {
            if (!originalMaterials.ContainsKey(obj))
                originalMaterials[obj] = rend.material;
            rend.material = newMat;
        }
    }

    void UnhighlightObject(GameObject obj)
    {
        if (obj == null) return;

        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null && originalMaterials.TryGetValue(obj, out Material original))
        {
            rend.material = original;
            originalMaterials.Remove(obj);
        }
    }
    #endregion

    #region OverlayPlacement
    /// <summary>
    /// Offsets a world position toward the camera based on the object's bounding size.
    /// Only works for objects with renderers.
    /// </summary>
    Vector3 OffsetInFrontOfObject(GameObject obj)
    {
        Renderer rend = obj.GetComponent<Renderer>();
        if (rend == null)
            return obj.transform.position;

        Bounds bounds = rend.bounds;
        float zOffset = bounds.size.z * 0.5f + 0.01f; // 0.001f is a small margin to push line in front
        return obj.transform.position + new Vector3(0, 0, -zOffset);
    }

    Vector3 OffsetZFrom(Vector3 worldPos, GameObject referenceObj)
    {
        Renderer rend = referenceObj.GetComponent<Renderer>();
        if (rend == null) return worldPos;

        Bounds bounds = rend.bounds;
        float zOffset = bounds.size.z * 0.5f + 0.01f;
        return worldPos + new Vector3(0, 0, -zOffset);
    }
    #endregion
}
