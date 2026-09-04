using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    [SerializeField] private float duration = 10f;

    [Header("Events")]
    [SerializeField] private UnityEvent onTimeOut;

    private Coroutine timerCoroutine;

    public void StartTimer()
    {
        if (duration <= 0f)
        {
            onTimeOut?.Invoke();
            return;
        }

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        timerCoroutine = StartCoroutine(WaitForTimeOut());
    }

    private IEnumerator WaitForTimeOut()
    {
        yield return new WaitForSeconds(duration);

        timerCoroutine = null;
        onTimeOut?.Invoke();
    }
}
