using Assets.Scripts.Networking;
using System;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game
{
    /// <summary>
    /// Event for game status changing.
    /// </summary>
    public class EventGameStatus : EventArgs
    {
        public GAME_STATE PreviousState;
        public GAME_STATE CurrentState;
    }

    /// <summary>
    /// Represents a game state.
    /// </summary>
    public enum GAME_STATE { Loading, Play, Pause, Menu, GameOver, WarmUp }

    /// <summary>
    /// Abstract class for the game manager.
    /// </summary>
    public abstract class AGameManager : NetworkBehaviour
    {
        /// <summary>
        /// Singleton instance of the Manager.
        /// </summary>
        public static AGameManager Instance { get; private set; }

        /// <summary>
        /// the current game state.
        /// </summary>
        public GAME_STATE GameState
        {
            get { return _gameState; }
            private set { _gameState = value; }
        }

        /// <summary>
        /// Event triggered when the game states changes.
        /// </summary>
        public event EventHandler<EventGameStatus> OnGameStateChanged;

        private GAME_STATE _gameState;

        /// <summary>
        /// Called when the object awakens. Sets up the singleton.
        /// </summary>
        virtual protected void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        /// <summary>
        /// Sets the game state and triggers the corresponding event.
        /// </summary>
        /// <param name="state"></param>
        [Server]
        protected void SetGameState(GAME_STATE state)
        {
            var prevState = GameState;
            GameState = state;
            if (OnGameStateChanged != null)
                OnGameStateChanged.Invoke(this, new EventGameStatus { CurrentState = state, PreviousState = prevState });
            RpcSetGameState(state);
        }

        /// <summary>
        /// Replication to client of the game state.
        /// </summary>
        /// <param name="state"></param>
        [ClientRpc]
        protected void RpcSetGameState(GAME_STATE state)
        {
            var prevState = GameState;
            GameState = state;
            if (OnGameStateChanged != null)
                OnGameStateChanged.Invoke(this, new EventGameStatus { CurrentState = state, PreviousState = prevState });
        }

        /// <summary>
        /// Changes the scene, server side.
        /// </summary>
        /// <param name="name"></param>
        public virtual void ChangeScene(string name, string description, float minLoadTime)
        {
            RpcChangeScene(description, minLoadTime);
            //LobbyManager.Instance.ChangeScene(name);
            StartCoroutine(ChangeSceneInternal(name, minLoadTime));
        }

        public virtual void ChangeScene(string name)
        {
            RpcChangeScene("Good luck", 0);
            LobbyManager.Instance.ChangeScene(name);
        }

        private IEnumerator ChangeSceneInternal(string name, float time)
        {
            yield return new WaitForSeconds(time);
            LobbyManager.Instance.ChangeScene(name);
        }

        /// <summary>
        /// Scene changing replication on client.
        /// </summary>
        [ClientRpc]
        protected virtual void RpcChangeScene(string description, float minLoadtime)
        {
            LobbyManager.Instance.ShowLoadingScreen(true, description, minLoadtime);
        }
    }
}