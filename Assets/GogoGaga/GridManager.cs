using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single cell in the grid.
/// </summary>
public class Node
{
    public Vector2Int GridPos;
    public bool Walkable;
    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;
    public Node Parent;

    public Node(Vector2Int gridPos, bool walkable)
    {
        GridPos = gridPos;
        Walkable = walkable;
    }
}

/// <summary>
/// Builds a grid based on the bounds of the GameObject's MeshRenderer.
/// Width and height are derived from the mesh size along X and Z; Y is ignored.
/// </summary>
//[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class GridManager : MonoBehaviour
{
    [Tooltip("World‑space width / height of each square cell (in metres)")]
    [SerializeField] private float cellSize = 1f;

    [Tooltip("Physics layers treated as obstacles during grid generation")]
    [SerializeField] private LayerMask obstacles;

    private Node[,] _grid;
    private int _width;
    private int _height;
    private Vector3 _origin; // bottom‑left corner of the mesh bounds in world space

    #region Public API
    public int Width => _width;
    public int Height => _height;
    public float CellSize => cellSize;
    public Vector3 Origin => _origin;
    public Node[,] Grid => _grid;

    public Node GetNode(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= _width || gridPos.y >= _height) return null;
        return _grid[gridPos.x, gridPos.y];
    }

    /// <summary>
    /// Centre of the requested grid cell in world space.
    /// </summary>
    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return _origin + new Vector3((gridPos.x + 0.5f) * cellSize, 0, (gridPos.y + 0.5f) * cellSize);
    }

    /// <summary>
    /// Converts a world position to the containing grid index.
    /// </summary>
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        Vector3 local = worldPos - _origin;
        int x = Mathf.FloorToInt(local.x / cellSize);
        int y = Mathf.FloorToInt(local.z / cellSize);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Enumerates the 4‑way orthogonal neighbours of <paramref name="node"/>.
    /// </summary>
    public IEnumerable<Node> GetNeighbours(Node node)
    {
        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        foreach (var dir in dirs)
        {
            var neighbour = GetNode(node.GridPos + dir);
            if (neighbour != null && neighbour.Walkable)
                yield return neighbour;
        }
    }
    #endregion

    private void Awake()
    {
        MeshRenderer mr = GetComponent<MeshRenderer>();
        Bounds bounds = mr.bounds; // world‑space bounds

        // Compute grid dimensions from the mesh size along X & Z.
        _width = Mathf.Max(1, Mathf.FloorToInt(bounds.size.x / cellSize));
        _height = Mathf.Max(1, Mathf.FloorToInt(bounds.size.z / cellSize));

        _origin = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);

        _grid = new Node[_width, _height];

        // Populate grid and mark walkable state using Physics overlap test.
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector3 cellCenter = GridToWorld(new Vector2Int(x, y));
                bool walkable = !Physics.CheckBox(
                    cellCenter,
                    Vector3.one * (cellSize * 0.5f),
                    Quaternion.identity,
                    obstacles
                );
                _grid[x, y] = new Node(new Vector2Int(x, y), walkable);
            }
        }
    }
}