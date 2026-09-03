using UnityEngine;
using UnityEditor;

//[ExecuteInEditMode]
[RequireComponent(typeof(GridManager))]
[RequireComponent(typeof(MeshRenderer))]
public class GridVisualizer : MonoBehaviour
{
    private Vector3 cornerPosition; // perhaps not start in the origin though?

    private float cellSizeX;
    private float cellSizeY;

    [SerializeField] private Material gizmoMat;
    [SerializeField] private Material gizmoMatIllegal;
    [SerializeField] private Material gizmoMatDiscouraged;
    [SerializeField] private LayerMask obstacles;

    [Tooltip("How many cells the X direction will be diveded in")]
    [SerializeField] private float cellsX = 10;

    [Tooltip("How many cells the X direction will be diveded in")]
    [SerializeField] private float cellsY = 10;


    [ContextMenu("Draw Grid Gizmos")]
    private void RefreshSceneView()
    {
        DrawBalls();
        Debug.Log("Grid visualization updated.");
        SceneView.RepaintAll(); // Force update in Scene view
    }

    private void DrawBalls()//OnDrawGizmosSelected()
    {
        updateSizeFromMesh();

        Gizmos.color = Color.cyan;

        //Destroy former children:
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        Debug.Log($"cornerPosition:{cornerPosition}");

        for (int x = 0; x < cellsX; x++)
        {
            for (int y = 0; y < cellsY; y++)
            {
                Vector3 newPosition = cornerPosition + new Vector3(x * cellSizeX, y * cellSizeY, 0);
                Vector3 center = newPosition + new Vector3(0.5f * cellSizeX, 0.5f * cellSizeY, 0);
                //Gizmos.DrawSphere(center, Mathf.Min(cellSizeX, cellSizeZ) * 0.1f);

                float size = Mathf.Min(cellSizeX, cellSizeY) * 0.5f;

                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.localScale = new Vector3(size, size, size);
                sphere.transform.position = center;
                sphere.transform.SetParent(this.transform);

                Renderer sphereRenderer = sphere.GetComponent<Renderer>();

                //sphereRenderer.material = gizmoMat;
                sphereRenderer.material = MaskCost(center);
            }
        }
    }

    private void updateSizeFromMesh()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        Bounds bounds = mr.bounds; // world‑space bounds

        cellSizeX = bounds.size.x / cellsX; //unity: [m/cell]
        cellSizeY = bounds.size.y / cellsY; //unity: [m/cell]

        cornerPosition = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
    }

    private Material MaskCost(Vector3 centerPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapPointAll(centerPosition);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag("Connectable"))
            {
                return gizmoMatIllegal;
            }
        }
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.CompareTag("Product"))
            {
                return gizmoMatDiscouraged;
            }
        }
        return gizmoMat;
    }
} 

