using UnityEngine;
using UnityEngine.Events;

public class DamageDetector : MonoBehaviour
{
    [SerializeField] private float minimumImpactVelocity = 2f;
    [SerializeField] private float damageEventCooldown = 1f;

    [Header("Events")]
    [SerializeField] private UnityEvent[] damageEvents;

    private int nextDamageEventIndex;
    private float nextDamageEventTime;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= minimumImpactVelocity)
        {
            Debug.Log($"Hard impact detected: {collision.relativeVelocity.magnitude:F2} m/s");
            TriggerNextDamageEvent();
        }
    }

    [ContextMenu("Trigger Next Damage Event")]
    private void TriggerNextDamageEvent()
    {
        if (damageEvents == null || nextDamageEventIndex >= damageEvents.Length || Time.time < nextDamageEventTime)
        {
            return;
        }

        UnityEvent damageEvent = damageEvents[nextDamageEventIndex];
        nextDamageEventIndex++;
        nextDamageEventTime = Time.time + damageEventCooldown;
        damageEvent?.Invoke();
    }
}
