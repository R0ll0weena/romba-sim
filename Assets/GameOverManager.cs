using UnityEngine;
using System;

public class GameOverManager : MonoBehaviour
{
    public static event EventHandler<GameOverEventArgs> OnGameOver; //  Event triggered when game is over
    public class GameOverEventArgs : EventArgs
    {
        public bool PlayerWon;
    }

    public static void TriggerGameOver(bool playerWon)
    {
        OnGameOver?.Invoke(null, new GameOverEventArgs
        {
            PlayerWon = playerWon
        }); //  Notify all listeners, create and pass on the event args
        Debug.Log("Game over manager says all dirts have been destroyed!");
    }
}