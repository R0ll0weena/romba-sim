using UnityEngine;
using System;

public class Collectable : MonoBehaviour
{
    [SerializeField] private GameObject onCollectEffect;
    public static event EventHandler<EventArgs> OnChildDestroyed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Destroy the collectible
            Destroy(gameObject);
            // instantiate the particle effect
            Instantiate(onCollectEffect, transform.position, transform.rotation);
        }
    }

    void OnDestroy()
    {
        OnChildDestroyed?.Invoke(this, EventArgs.Empty);  // Publisher event that notifies parent when destroyed
    }
}
