using UnityEngine;
using System;

public class GameOverTimer : MonoBehaviour
{
    [SerializeField] private float countdownTime = 60f; // Time in seconds
    public float remainingTime;
    private bool timerRunning = true;
    private int lastPrintedSecond;

    private void Start()
    {
        remainingTime = countdownTime;
        lastPrintedSecond = Mathf.CeilToInt(remainingTime);
        PrintTime();
    }

    void OnEnable()
    {
        GameOverManager.OnGameOver += StopTimer; //  Subscribe to event
    }

    void OnDisable()
    {
        GameOverManager.OnGameOver -= StopTimer; //  Unsubscribe
    }

    private void Update()
    {
        if (timerRunning)
        {
            remainingTime -= Time.deltaTime;
            int currentSecond = Mathf.CeilToInt(remainingTime);

            // Print only when the second changes
            if (currentSecond < lastPrintedSecond)
            {
                lastPrintedSecond = currentSecond;
                PrintTime();
            }

            if (remainingTime <= 0)
            {
                remainingTime = 0;
                timerRunning = false;
                TriggerGameOver();
            }
        }
    }

    private void PrintTime()
    {
        Debug.Log("Time remaining: " + lastPrintedSecond + " seconds");
    }

    private void TriggerGameOver()
    {
        // Trigger Game Over event with PlayerWon = false (player lost)
        GameOverManager.TriggerGameOver(false);
    }

    private void StopTimer(object sender, GameOverManager.GameOverEventArgs e)
    {
        timerRunning = false;
    }

    public void ResetTimer()
    {
        remainingTime = countdownTime;
        lastPrintedSecond = Mathf.CeilToInt(remainingTime);
        timerRunning = true;
        PrintTime();
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }
}
