/*using UnityEngine;

public class LineInputManager : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private GameObject startPoint;
    [SerializeField] private GameObject endPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        line.enabled = true;
        line.positionCount = 2; // 2 points 
    }

    // Update is called once per frame
    void Update()
    {
        showLine();
    }

    private void showLine()
    {
       

        //if (Input.GetMouseButton(0))
        //{
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // converted from pixel space to world space
        endPoint.transform.position = mousePos;


        line.SetPosition(0, startPoint.transform.position);
        line.SetPosition(1, mousePos);

        Debug.Log("startPoint.position" + startPoint.transform.position);
        Debug.Log("mousePosition" + mousePos);
        //}
    }
}*/

using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MouseLineRenderer : MonoBehaviour
{
    //public Vector3 startPoint = Vector3.zero; // You can also make this a Transform reference
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject startPointObj;
    [SerializeField] private GameObject endPoint;

    [SerializeField] private Camera mainCamera;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mainCamera = Camera.main;

        // Set line properties
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        // Optional: set line appearance
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
    }

    void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, Mathf.Abs(mainCamera.transform.position.z)));
        Vector3 startPoint = startPointObj.transform.position;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, mouseWorldPos);
    }
}
