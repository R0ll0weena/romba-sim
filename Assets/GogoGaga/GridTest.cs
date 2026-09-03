using UnityEngine;
using System.Collections.Generic;

namespace Pathfinder
{
    public class GridTest : MonoBehaviour
    {
        [SerializeField] GridManager gmr;
        Pathfinding pathFinder;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] LineRenderer lineRenderer;
        void Start()
        {
            pathFinder = new Pathfinding(gmr);
        }

        private Vector3 startPosition;
        private Vector3 endPosition;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (startPosition == Vector3.zero)
                {
                    startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
                else if (endPosition == Vector3.zero)
                {
                    endPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    FindPath();
                }
            }
        }

        private void FindPath()
        {
            // Assuming you have a PathFinder class that takes start and end positions and returns a List<Vector3>
            List<Vector3> path = pathFinder.FindPath(startPosition, endPosition);
            Debug.Log($"path:{path}");

            if (path != null){ 
                foreach (Vector3 vector in path)
                {
                    Debug.Log(vector);
                    DrawBall(vector);
                }
                lineRenderer = pathFinder.DrawPath(path, lineRenderer);
                GameObject.Instantiate(lineRenderer);
            }
            
            // Do something with the path, like visualize it in the scene

            //reset for next path
            startPosition = Vector3.zero;
            endPosition = Vector3.zero;
        }

        private void DrawBall(Vector3 center)
		{
            float size = 0.01f;

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(size, size, size);
            sphere.transform.position = center;
            sphere.transform.SetParent(this.transform);

            Renderer sphereRenderer = sphere.GetComponent<Renderer>();
        }
    }
}
