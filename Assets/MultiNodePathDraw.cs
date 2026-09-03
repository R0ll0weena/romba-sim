using UnityEngine;
using System.Collections.Generic;

namespace Pathfinder { 
    public class MultiNodePathDraw : MonoBehaviour
    {
        //[SerializeField] private Pathfinding pathfinder;
        /*
        Pathfinding pathfinder;
        [SerializeField] private LineRenderer lineRen;
        void Start() => pathfinder = FindFirstObjectByType<Pathfinding>();
        public void MoveTo(Unit target)
        {
            List<Vector3> path = pathfinder.FindPath(transform.position, target.transform.position);

            LineRenderer go = InstantiateSplineFromPath(path, lineRen);


        }

        public LineRenderer InstantiateSplineFromPath(List<Vector3> pathPoints, LineRenderer lineRen)
        {
            if (pathPoints == null || pathPoints.Count == 0 || lineRen == null)
                return null;

            LineRenderer splineInstance = GameObject.Instantiate(lineRen);
            var line = splineInstance.GetComponent<LineRenderer>();

            if (line == null)
            {
                Debug.LogWarning("Spline prefab is missing a LineRenderer component.");
                GameObject.Destroy(splineInstance);
                return null;
            }

            line.positionCount = pathPoints.Count;
            line.SetPositions(pathPoints.ToArray());

            return splineInstance;
        }*/


    }
}