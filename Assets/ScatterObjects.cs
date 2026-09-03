using UnityEngine;
using System;

public class ScatterObjects : MonoBehaviour
{
    [SerializeField] private Collectable objectToScatter; // Prefab to scatter
    [SerializeField] private GameObject[] floorsToScatterOn;
    [SerializeField, Min(0f)] private float edgePadding = 0.02f;
    public int remainingChildren = 30; // Number of objects
    //public Vector2 areaSize = new Vector2(10, 10); // Size of the plane


    void Awake()
    {
        Scatter();
        Collectable.OnChildDestroyed += HandleChildDestroyed; //  Subscribe to static event (class name, subscribing to static event!)
    }

    private void Start()
    {
        disableFloorColliders();
    }

    private void disableFloorColliders()
    {
        for (int i = 0; i < floorsToScatterOn.Length; i++)
        {
            if (floorsToScatterOn[i] != null)
            {
                Debug.LogError("Floor " + i + " is not assigned on " + gameObject.name + ".");
                Collider floorCollider = floorsToScatterOn[i].GetComponentInChildren<Collider>();
                if (floorCollider != null)
                {
                    floorCollider.enabled = false; // turn off after spawning, more reliable to use one single plane for player control
                }
            }
        }
    }

    void Scatter()
    {
        if (floorsToScatterOn == null || floorsToScatterOn.Length == 0)
        {
            Debug.LogError("No floors assigned on " + gameObject.name + ". Assign at least one floor prefab to define the scatter areas.");
            return;
        }

        Collider[] floorColliders = new Collider[floorsToScatterOn.Length];
        for (int i = 0; i < floorsToScatterOn.Length; i++)
        {
            if (floorsToScatterOn[i] == null)
            {
                Debug.LogError("Floor " + i + " is not assigned on " + gameObject.name + ".");
                return;
            }

            floorColliders[i] = floorsToScatterOn[i].GetComponentInChildren<Collider>();
            if (floorColliders[i] == null)
            {
                Debug.LogError("No Collider found on " + floorsToScatterOn[i].name + ". Add one to define the scatter area.");
                return;
            }
        }

        Collectable newObject;
        for (int i = 0; i < remainingChildren; i++)
        {
            Collider areaCollider = floorColliders[i % floorColliders.Length];
            Bounds areaBounds = areaCollider.bounds;
            newObject = Instantiate(objectToScatter, areaBounds.center, Quaternion.identity, transform);

            float randomScale = UnityEngine.Random.Range(1f, 3f);
            newObject.transform.localScale *= randomScale;

            Bounds objectBounds = GetObjectBounds(newObject);
            Vector3 boundsOffset = objectBounds.center - newObject.transform.position;
            Vector3 minimum = areaBounds.min + objectBounds.extents - boundsOffset;
            Vector3 maximum = areaBounds.max - objectBounds.extents - boundsOffset;
            minimum += new Vector3(edgePadding, 0f, edgePadding);
            maximum -= new Vector3(edgePadding, 0f, edgePadding);

            newObject.transform.position = new Vector3(
                UnityEngine.Random.Range(minimum.x, maximum.x),
                areaBounds.max.y - (objectBounds.min.y - newObject.transform.position.y),
                UnityEngine.Random.Range(minimum.z, maximum.z)
            );
            
        }
    }

    private Bounds GetObjectBounds(Collectable objectToMeasure)
    {
        Collider[] colliders = objectToMeasure.GetComponentsInChildren<Collider>();
        if (colliders.Length == 0)
        {
            Renderer[] renderers = objectToMeasure.GetComponentsInChildren<Renderer>();
            Bounds rendererBounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                rendererBounds.Encapsulate(renderers[i].bounds);
            }
            return rendererBounds;
        }

        Bounds bounds = colliders[0].bounds;
        for (int i = 1; i < colliders.Length; i++)
        {
            bounds.Encapsulate(colliders[i].bounds);
        }
        return bounds;
    }

    private void ReactToProgress_OnChildDestroyed(object sender)
    {
        checkGameOver();

    }

    private Vector3 getAreaSize()
	{
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("No Collider found on " + gameObject.name + ". Add one to define the scatter area.");
            return new Vector3(10,0,10);
        }
        return collider.bounds.size; // Use collider size
    }

    public void checkGameOver()
	{
        if (transform.childCount == 0) //  Check if all objects are gone
        {
            //GameOverManager.TriggerGameOver(); //  Fire the Game Over event
            Debug.Log("Scatter Object Plane says all dirts have been destroyed!");
        }
    }

    private void HandleChildDestroyed(object sender, EventArgs e)
    {
        remainingChildren--; //  Reduce count when a child is destroyed

        if (remainingChildren <= 0) //  If no children left, trigger game over
        {
            GameOverManager.TriggerGameOver(true);
        }
    }

    void OnDestroy()
    {
        Collectable.OnChildDestroyed -= HandleChildDestroyed; //  Unsubscribe to prevent memory leaks
    }
}