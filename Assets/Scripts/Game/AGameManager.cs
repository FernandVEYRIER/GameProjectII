using System;
using UnityEngine.Networking;

namespace Assets.Scripts.Game
{
    /// <summary>
    /// Event for game status changing.
    /// </summary>
    public class EventGameStatus : EventArgs
    {
        public AGameManager.GAME_STATE PreviousState;
        public AGameManager.GAME_STATE CurrentState;
    }

    /// <summary>
    /// Abstract class for the game manager.
    /// </summary>
    public abstract class AGameManager : NetworkBehaviour
    {
        /// <summary>
        /// Represents a game state.
        /// </summary>
        public enum GAME_STATE { Play, Pause, Menu, GameOver }

        /// <summary>
        /// the current game state.
        /// </summary>
        public GAME_STATE GameState { get; private set; }

        /// <summary>
        /// Event triggered when the game states changes.
        /// </summary>
        public event EventHandler<EventGameStatus> OnGameStateChanged;

        /// <summary>
        /// Sets the game state and triggers the corresponding event.
        /// </summary>
        /// <param name="state"></param>
        protected void SetGameState(GAME_STATE state)
        {
            var prevState = GameState;
            GameState = state;
            if (OnGameStateChanged != null)
                OnGameStateChanged.Invoke(this, new EventGameStatus { CurrentState = state, PreviousState = prevState });
        }
    }
}