using UnityEngine;
using System.Collections;

public class CatHit : MonoBehaviour
{
    [SerializeField] private string parameterName = "catHit"; // Name of the Animator parameter
    [SerializeField] private Animator animator;
    [SerializeField] private float resetTime = 0.1f;

    void Start()
    {
        //animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TriggerAnimation();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TriggerAnimation();
        }
    }

    private void TriggerAnimation()
    {
        if (animator != null)
        {
            animator.SetBool(parameterName, true); // Set parameter to true
            StartCoroutine(ResetAnimation()); // Start coroutine to reset it
            //animator.SetBool(parameterName, false); // Set parameter to true
        }
    }

    private IEnumerator ResetAnimation()
    {
        yield return new WaitForSeconds(resetTime);
        animator.SetBool(parameterName, false); // Reset the parameter after delay
    }
}
