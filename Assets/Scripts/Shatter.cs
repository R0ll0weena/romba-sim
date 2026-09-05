using UnityEngine;

public class Shatter : MonoBehaviour
{
    [SerializeField] private GameObject shatterPrefab;

    private bool hasShattered;

    public void ShatterObject()
    {
        if (hasShattered || shatterPrefab == null)
        {
            return;
        }

        hasShattered = true;

        Transform objectTransform = transform;
        Renderer originalRenderer = GetComponent<Renderer>();
        Material originalMaterial = originalRenderer != null ? originalRenderer.sharedMaterial : null;
        GameObject replacement = Instantiate(
            shatterPrefab,
            objectTransform.position,
            objectTransform.rotation,
            objectTransform.parent);

        replacement.transform.localScale = objectTransform.localScale;

        if (originalMaterial != null)
        {
            foreach (Renderer replacementRenderer in replacement.GetComponentsInChildren<Renderer>(true))
            {
                replacementRenderer.sharedMaterial = originalMaterial;
            }
        }

        Destroy(gameObject);
    }
}
