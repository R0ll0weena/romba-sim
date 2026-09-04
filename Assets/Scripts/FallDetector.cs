using UnityEngine;
using UnityEngine.Events;

public class FallDetector : MonoBehaviour
{
    [SerializeField] private float minimumImpactVelocity = 2f;

    [Header("Events")]
    [SerializeField] private UnityEvent onFallDetected;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= minimumImpactVelocity)
        {
            Debug.Log($"Hard impact detected: {collision.relativeVelocity.magnitude:F2} m/s");

            onFallDetected?.Invoke();
        }
    }

    [ContextMenu("Trigger Fall Event")]
    private void TriggerFallEvent()
    {
        onFallDetected?.Invoke();
    }
}