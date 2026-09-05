using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageDetector : MonoBehaviour
{
    [SerializeField] private float minimumImpactVelocity = 2f;
    [SerializeField] private float damagePerImpact = 25f;
    [SerializeField] private float damageEventCooldown = 1f;

    [SerializeField] private Color damageColor = Color.black;
    private Material damageMaterial;

    private const float MaximumHealth = 100f;
    private float currentHealth = MaximumHealth;
    private int damageLevel;
    private readonly Queue<int> pendingDamageLevels = new Queue<int>();
    private Coroutine damageEventCoroutine;
    private float nextDamageEventTime;

    [Header("Events")]
    [SerializeField] private UnityEvent takeDamage1;
    [SerializeField] private UnityEvent takeDamage2;
    [SerializeField] private UnityEvent takeDamage3;
    [SerializeField] private UnityEvent takeDamage4;
    [SerializeField] private UnityEvent debugEvent;

    private void Awake()
    {
        Renderer objectRenderer = GetComponent<Renderer>();
        damageMaterial = objectRenderer != null ? objectRenderer.material : null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude >= minimumImpactVelocity)
        {
            Debug.Log($"Hard impact detected: {collision.relativeVelocity.magnitude:F2} m/s");

            ApplyDamage(damagePerImpact);
        }
    }

    [ContextMenu("Apply Damage")]
    private void ApplyTestDamage()
    {
        ApplyDamage(damagePerImpact);
    }

    [ContextMenu("Trigger Debug Event")]
    private void TriggerDebugEvent()
    {
        debugEvent?.Invoke();
    }

    private void ApplyDamage(float damage)
    {
        if (currentHealth <= 0f || damage <= 0f || Time.time < nextDamageEventTime)
        {
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        int newDamageLevel = Mathf.Clamp(Mathf.FloorToInt((MaximumHealth - currentHealth) / 25f), 0, 4);

        while (damageLevel < newDamageLevel)
        {
            damageLevel++;
            pendingDamageLevels.Enqueue(damageLevel);
        }

        if (damageEventCoroutine == null)
        {
            damageEventCoroutine = StartCoroutine(InvokeDamageEvents());
        }
    }

    private IEnumerator InvokeDamageEvents()
    {
        while (pendingDamageLevels.Count > 0)
        {
            float waitTime = nextDamageEventTime - Time.time;
            if (waitTime > 0f)
            {
                yield return new WaitForSeconds(waitTime);
            }

            InvokeDamageEvent(pendingDamageLevels.Dequeue());
            nextDamageEventTime = Time.time + damageEventCooldown;
        }

        damageEventCoroutine = null;
    }

    private void InvokeDamageEvent(int level)
    {
        switch (level)
        {
            case 1:
                takeDamage1?.Invoke();
                break;
            case 2:
                takeDamage2?.Invoke();
                break;
            case 3:
                takeDamage3?.Invoke();
                break;
            case 4:
                takeDamage4?.Invoke();
                break;
        }
    }

    public void turnBurnt()
    {
        if (damageMaterial != null)
        {
            damageMaterial.color = damageColor;
        }
    }
}