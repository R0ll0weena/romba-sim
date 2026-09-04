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
        GameObject replacement = Instantiate(
            shatterPrefab,
            objectTransform.position,
            objectTransform.rotation,
            objectTransform.parent);

        replacement.transform.localScale = objectTransform.localScale;
        Destroy(gameObject);
    }
}
