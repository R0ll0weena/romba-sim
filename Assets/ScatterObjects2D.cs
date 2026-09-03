using UnityEngine;
using System;

public class ScatterObjects2D : MonoBehaviour
{
    //[SerializeField] private GameObject objectToScatter; // Prefab to scatter
    public int remainingChildren = 30; // Number of objects
    //public Vector2 areaSize = new Vector2(10, 10); // Size of the plane

    public GameObject objectToSpawn;
    public LayerMask collisionMask; // Set this in the Inspector to include objects you want to avoid
    public float spawnRadius = 1f;
    public int maxAttempts = 10;
    Vector2 areaSize;

    void Start()
    {
        Scatter();
    }

    

    void SpawnObject()
    {
        Vector2 spawnPosition = GetNewSpawnPosition();

        int attempts = 0;
        while (attempts < maxAttempts)
        {
            // Check if the spawn area is free
            Collider2D hit = Physics2D.OverlapCircle(spawnPosition, spawnRadius, collisionMask);

            if (hit == null) // No collision detected, spawn the object
            {
                GameObject newObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
                newObject.transform.parent = transform;
                return;
            }

            // Try a new position (you need logic for generating new positions)
            spawnPosition = GetNewSpawnPosition();
            
            attempts++;
        }

        Debug.LogWarning("Failed to find a valid spawn location after " + maxAttempts + " attempts.");
    }

    Vector2 GetNewSpawnPosition()
	{
        return new Vector2(
                UnityEngine.Random.Range(-areaSize.x / 2, areaSize.x / 2),
                UnityEngine.Random.Range(-areaSize.y / 2, areaSize.y / 2));
    }

    void Scatter()
    {
        //Vector2 areaSize = getAreaSize();
        areaSize = getAreaSize();
        //Vector3 size = objectToSpawn.GetComponent<Collider2D>().bounds;
        //spawnRadius = Mathf.Max(size.x, size.y);
        

        for (int i = 0; i < remainingChildren; i++)
        {
            /*Vector3 randomPosition = new Vector3(
                UnityEngine.Random.Range(-areaSize.x / 2, areaSize.x / 2),
                transform.position.y, // Keep Y at 0 (adjust if needed)
                UnityEngine.Random.Range(-areaSize.y / 2, areaSize.y / 2)
            );*/

            SpawnObject();

            /*Vector2 randomPosition = new Vector2(
                UnityEngine.Random.Range(-areaSize.x / 2, areaSize.x / 2),
                UnityEngine.Random.Range(-areaSize.y / 2, areaSize.y / 2)
            );*/

            //GameObject newObject = Instantiate(objectToScatter, randomPosition, Quaternion.identity);
            //newObject.transform.parent = transform;

            //float randomScale = UnityEngine.Random.Range(1f, 3f);
            //newObject.transform.localScale *= randomScale;
        }
    }

    

    private Vector2 getAreaSize()
    {
        Collider2D collider = GetComponent<Collider2D>();
        Debug.Log("Collider found : " + collider.bounds.size);
        if (collider == null)
        {
            Debug.LogError("No Collider found on " + gameObject.name + ". Add one to define the scatter area.");
            return new Vector2(10, 10);
        }
        return collider.bounds.size; // Use collider size
    }
}