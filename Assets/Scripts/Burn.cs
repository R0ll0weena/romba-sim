using System.Collections;
using UnityEngine;

public class Burn : MonoBehaviour
{
    [Header("Initial Effects")]
    [SerializeField] private ParticleSystem firstParticleSystem;
    [SerializeField] private ParticleSystem secondParticleSystem;
    [SerializeField] private float initialEffectDuration = 2f;

    [Header("Burning Effects")]
    [SerializeField] private ParticleSystem additionalParticleSystem;
    [SerializeField] private AudioSource soundEffect;
    [SerializeField] private float burningEffectDuration = 3f;
    [SerializeField] private Color burntColor = new Color(0.08f, 0.08f, 0.08f, 1f);

    [Header("Material")]
    [SerializeField] private GameObject materialObject;

    private Renderer materialRenderer;
    private Material objectMaterial;
    private Coroutine burnCoroutine;

    private void Awake()
    {
        materialRenderer = materialObject != null
            ? materialObject.GetComponentInChildren<Renderer>(true)
            : null;

        if (materialRenderer != null && materialRenderer.sharedMaterial != null)
        {
            objectMaterial = new Material(materialRenderer.sharedMaterial);
            materialRenderer.material = objectMaterial;
        }
    }

    public void burn()
    {
        if (burnCoroutine == null)
        {
            burnCoroutine = StartCoroutine(BurnSequence());
        }
    }

    private IEnumerator BurnSequence()
    {
        PlayParticleSystem(firstParticleSystem);
        PlayParticleSystem(secondParticleSystem);
        yield return new WaitForSeconds(Mathf.Max(0f, initialEffectDuration));

        PlayParticleSystem(additionalParticleSystem);
        if (soundEffect != null)
        {
            soundEffect.Play();
        }

        Color startingColor = objectMaterial != null ? objectMaterial.color : Color.white;
        float elapsedTime = 0f;
        float duration = Mathf.Max(0f, burningEffectDuration);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float colorProgress = duration > 0f ? Mathf.Clamp01(elapsedTime / duration) : 1f;

            if (objectMaterial != null)
            {
                objectMaterial.color = Color.Lerp(startingColor, burntColor, colorProgress);
            }

            yield return null;
        }

        if (objectMaterial != null)
        {
            objectMaterial.color = burntColor;
        }

        StopParticleSystem(firstParticleSystem);
        StopParticleSystem(secondParticleSystem);
        StopParticleSystem(additionalParticleSystem);

        if (soundEffect != null)
        {
            soundEffect.Stop();
        }

        burnCoroutine = null;
    }

    private static void PlayParticleSystem(ParticleSystem particleSystem)
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    private static void StopParticleSystem(ParticleSystem particleSystem)
    {
        if (particleSystem != null)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
