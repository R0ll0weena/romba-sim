using UnityEngine;
using System.Collections.Generic;

public class LineConnectorManager : MonoBehaviour
{
    [Header("Line Settings")]
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
    private LineRenderer previewLine;

    private Dictionary<GameObject, LineRenderer> activeLines = new();
    private Dictionary<GameObject, Material> originalMaterials = new();

    // Offset to draw lines slightly closer to the camera
    private const float zOffsetTowardsCamera = -0.1f;

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
            UpdatePreviewLineToMouse();
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
                CreatePreviewLineFrom(selectedObject);
            }
            else
            {
                if (clicked == selectedObject)
                {
                    CancelPreviewLine();
                    UnhighlightObject(selectedObject);
                    selectedObject = null;
                    return;
                }

                CreateConnection(selectedObject, clicked);
                UnhighlightObject(selectedObject);
                selectedObject = null;
                CancelPreviewLine();
            }
        }
    }

    void HandleCancel()
    {
        if (selectedObject != null && (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape)))
        {
            UnhighlightObject(selectedObject);
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

    void CreateConnection(GameObject fromObj, GameObject toObj)
    {
        RemoveConnection(fromObj);
        RemoveConnection(toObj);

        LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
        line.material = lineMaterial;
        line.startWidth = line.endWidth = lineWidth;
        line.positionCount = 2;
        line.useWorldSpace = true;

        // Move line positions slightly closer to the camera
        Vector3 fromPos = OffsetInFrontOfObject(fromObj);
        Vector3 toPos = OffsetInFrontOfObject(toObj);

        line.SetPosition(0, fromPos);
        line.SetPosition(1, toPos);

        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        line.receiveShadows = false;

        activeLines[fromObj] = line;
        activeLines[toObj] = line;
    }

    void RemoveConnection(GameObject obj)
    {
        if (activeLines.TryGetValue(obj, out LineRenderer oldLine))
        {
            foreach (var key in new List<GameObject>(activeLines.Keys))
            {
                if (activeLines[key] == oldLine)
                    activeLines.Remove(key);
            }
            Destroy(oldLine.gameObject);
        }
    }

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

    void CancelPreviewLine()
    {
        if (previewLine != null)
            Destroy(previewLine.gameObject);
        previewLine = null;
    }

    void ApplySelectionHighlight(GameObject obj)
    {
        SetMaterial(obj, selectionHighlightMaterial);
    }

    void ApplyHoverHighlight(GameObject obj)
    {
        SetMaterial(obj, hoverHighlightMaterial);
    }

    void ClearHoverHighlight()
    {
        if (currentHoveredObject != null && currentHoveredObject != selectedObject)
        {
            UnhighlightObject(currentHoveredObject);
        }
        currentHoveredObject = null;
    }

    void SetMaterial(GameObject obj, Material newMat)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && newMat != null)
        {
            if (!originalMaterials.ContainsKey(obj))
                originalMaterials[obj] = renderer.material;
            renderer.material = newMat;
        }
    }

    void UnhighlightObject(GameObject obj)
    {
        if (obj == null) return;

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && originalMaterials.TryGetValue(obj, out Material original))
        {
            renderer.material = original;
            originalMaterials.Remove(obj);
        }
    }


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
        float zOffset = bounds.size.z * 0.5f + 0.001f; // 0.001f is a small margin to push line in front
        return obj.transform.position + new Vector3(0, 0, -zOffset);
    }

    Vector3 OffsetZFrom(Vector3 worldPos, GameObject referenceObj)
    {
        Renderer rend = referenceObj.GetComponent<Renderer>();
        if (rend == null) return worldPos;

        Bounds bounds = rend.bounds;
        float zOffset = bounds.size.z * 0.5f + 0.001f;
        return worldPos + new Vector3(0, 0, -zOffset);
    }

}
