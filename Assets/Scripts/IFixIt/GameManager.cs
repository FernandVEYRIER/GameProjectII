using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.IFixIt
{
    /// <summary>
    /// Game Manager for the I Fix It game.
    /// </summary>
    public class GameManager : AGameManager
    {
        public bool AllPlayersFinished { get; private set; }

        private List<Action> _gameActions = new List<Action>();

        private void Start()
        {
            _gameActions.Add(PlayScrew);
            _gameActions.Add(PlaySwipe);
            _gameActions.Add(PlayNail);

            if (isServer)
                StartCoroutine(WaitForPlayers());
        }

        private IEnumerator WaitForPlayers()
        {
            if (!isServer)
                yield break;

            Debug.Log("Server ready, waiting for players... " + NetworkServer.connections.Count);

            while (!LobbyManager.Instance.AreAllClientsReady)
                yield return null;

            SetGameState(GAME_STATE.Play);
            StartCoroutine(ChoseGame());
        }

        /// <summary>
        /// Choses a game to play randomly.
        /// </summary>
        /// <returns></returns>
        private IEnumerator ChoseGame()
        {
            while (GameState == GAME_STATE.Play)
            {
                while (!AllPlayersFinished)
                    yield return null;
                _gameActions[UnityEngine.Random.Range(0, _gameActions.Count)].Invoke();
            }
        }

        private void PlayScrew()
        {
            Debug.Log("Playing screw game");
            AllPlayersFinished = false;
            RpcPlayScrew();
        }

        private void PlayNail()
        {
            throw new NotImplementedException();
        }

        private void PlaySwipe()
        {
            throw new NotImplementedException();
        }

        [ClientRpc]
        private void RpcPlayScrew()
        {
            Debug.Log("Client playing screw");
        }
    }
}