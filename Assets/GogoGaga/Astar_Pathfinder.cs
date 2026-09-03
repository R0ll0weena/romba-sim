using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinder
{
    /// <summary>
    /// Holds grid data and provides helpers to translate between world and grid space.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        [Tooltip("# of cells in X (columns) and Y (rows)")]
        public Vector2Int GridSize = new Vector2Int(20, 20);

        //World‑space width/height of each square cell
        private Vector2 cellSize;

        [Tooltip("Physics layers treated as obstacles")]
        public LayerMask Obstacles;

        Vector3 cornerPosition;

        private Node[,] _grid;

        private void Awake()
        {
            updateSizeFromMesh();

            _grid = new Node[GridSize.x, GridSize.y]; //initiate the whole grid, x*y cells in total
            for (int x = 0; x < GridSize.x; x++)
            {
                for (int y = 0; y < GridSize.y; y++)
                {
                    var pos = GridToWorld(new Vector2Int(x, y));
                    bool walkable = true; /*!Physics.CheckBox(
                        pos,
                        Vector3.one * (cellSize.x * 0.5f),
                        Quaternion.identity,
                        Obstacles
                    );*/
                    _grid[x, y] = new Node(new Vector2Int(x, y), walkable); // initiate each cell
                }
            }
        }

        private void updateSizeFromMesh()
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();
            Bounds bounds = mr.bounds; // world‑space bounds

            cellSize.x = bounds.size.x / GridSize.x; //unity: [m/cell]
            cellSize.y = bounds.size.y / GridSize.y;

            cornerPosition = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
        }

        public Node GetNode(Vector2Int gridPos)
        {
            if (gridPos.x < 0 || gridPos.y < 0 || gridPos.x >= GridSize.x || gridPos.y >= GridSize.y) return null;
            return _grid[gridPos.x, gridPos.y];
        }

        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            //return new Vector3(gridPos.x * cellSize.x, 0, gridPos.y * cellSize.y) + new Vector3(cellSize.x * 0.5f, 0, cellSize.y * 0.5f);
            // origin of grid in world
            // origin to the specific cell
            // offset to center of cell
            Debug.Log($"gridpos: {gridPos}");
            Debug.Log($"cornerPosition: {cornerPosition}");
            Vector3 pos =  cornerPosition + 
                new Vector3(gridPos.x * cellSize.x, gridPos.y * cellSize.y, 0) +
                new Vector3(cellSize.x * 0.5f, cellSize.y * 0.5f, 0);
            Debug.Log($"converted to: {pos}");
            return pos;
        }


        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            Vector3 local = worldPos - new Vector3(cellSize.x * 0.5f, cellSize.y * 0.5f, 0) - cornerPosition;
            Debug.Log($"local Position in grid:{local}");
            int x = Mathf.FloorToInt(local.x / cellSize.x);
            int y = Mathf.FloorToInt(local.y / cellSize.y);
            Debug.Log($"grid Position in grid:{x};{y}");
            return new Vector2Int(x, y);
        }

        public IEnumerable<Node> GetNeighbours(Node node)
        {
            // 4‑directional movement only – guarantees 90° segments
            Vector2Int[] dirs =
            {
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

            foreach (var dir in dirs)
            {
                var neighbour = GetNode(node.GridPos + dir);
                if (neighbour != null && neighbour.Walkable)
                    yield return neighbour;
            }
        }
    }

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
    /// Performs A star pathfinding on the supplied GridManager.
    /// Add this alongside the GridManager on the same GameObject.
    /// </summary>
    [RequireComponent(typeof(GridManager))]
    public class Pathfinding
    {
        private GridManager _grid;

        /*void Awake()
        {
            _grid = GetComponent<GridManager>();
        }*/

        public Pathfinding(GridManager gmr)
		{
            _grid = gmr;

        }

        /// <summary>
        /// Returns a list of world‑space positions from startWorld to targetWorld, or null if blocked.
        /// </summary>
        public List<Vector3> FindPath(Vector3 startWorld, Vector3 targetWorld)
        {
            if (_grid == null)
            {
                throw new ArgumentNullException(nameof(_grid), "The variable cannot be null, assign reference in inspector");
            }
            var startNode = _grid.GetNode(_grid.WorldToGrid(startWorld));
            var targetNode = _grid.GetNode(_grid.WorldToGrid(targetWorld));
            if (startNode == null || targetNode == null || !targetNode.Walkable) return null;

            var openSet = new List<Node> { startNode };
            var closedSet = new HashSet<Node>();

            startNode.GCost = 0;
            startNode.HCost = GetHeuristic(startNode, targetNode);

            while (openSet.Count > 0)
            {
                Node current = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < current.FCost ||
                        (openSet[i].FCost == current.FCost && openSet[i].HCost < current.HCost))
                    {
                        current = openSet[i];
                    }
                }

                openSet.Remove(current);
                closedSet.Add(current);

                if (current == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                foreach (var neighbour in _grid.GetNeighbours(current))
                {
                    if (closedSet.Contains(neighbour)) continue;

                    int tentativeG = current.GCost + 10; // cost per orthogonal step

                    if (!openSet.Contains(neighbour) || tentativeG < neighbour.GCost)
                    {
                        neighbour.GCost = tentativeG;
                        neighbour.HCost = GetHeuristic(neighbour, targetNode);
                        neighbour.Parent = current;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }

            return null; // No valid route
        }

        int GetHeuristic(Node a, Node b)
        {
            // Manhattan distance (no diagonals)
            return (Mathf.Abs(a.GridPos.x - b.GridPos.x) + Mathf.Abs(a.GridPos.y - b.GridPos.y)) * 10;
        }

        List<Vector3> RetracePath(Node startNode, Node endNode)
        {
            var path = new List<Node>();
            var current = endNode;
            while (current != startNode)
            {
                path.Add(current);
                current = current.Parent;
            }
            path.Reverse();

            //convert Gridpath to worldPath
            var worldPath = new List<Vector3>(path.Count);
            foreach (var n in path)
                worldPath.Add(_grid.GridToWorld(n.GridPos) + Vector3.up * 0.1f); // slight Y offset for visibility

            return worldPath;
        }

        /// <summary>
        /// Handy helper to visualize the route using a LineRenderer.
        /// Pass in the LineRenderer you added to the scene.
        /// </summary>
        public LineRenderer DrawPath(List<Vector3> worldPath, LineRenderer lr)
        {
            if (worldPath == null || worldPath.Count == 0)
            {
                lr.positionCount = 0;
                return null;
            }
            lr.positionCount = worldPath.Count;
            lr.SetPositions(worldPath.ToArray());
            return lr;
        }
    }

    /*
    # Quick‑start usage
    1. Create an empty GameObject called "Grid" and attach GridManager and Pathfinding.
    2. Set GridSize and CellSize in the inspector.
    3. Create a physics layer called "Obstacle" and assign it to walls/blocks. Add that layer to the GridManager Obstacles mask.
    4. Drop a LineRenderer into the scene, assign a material and width, then reference it when calling DrawPath.
    5. From your selection or input code:

        var path = pathfinder.FindPath(startTransform.position, endTransform.position);
        pathfinder.DrawPath(path, lineRenderer);

    */
}
