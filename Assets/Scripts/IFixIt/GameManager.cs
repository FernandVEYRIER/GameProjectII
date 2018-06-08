using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.IFixIt
{
    /// <summary>
    /// Game Manager for the I Fix It game.
    /// </summary>
    public class GameManager : AGameManager
    {
        public struct PlayerStats
        {
            public string Name;
            public float Time;
            public int CurrentGameIdx;

            public bool IsEmpty()
            {
                return string.IsNullOrEmpty(Name);
            }
        }

        public class PlayerStatsSync : SyncListStruct<PlayerStats>
        {
        }

        public bool AllPlayersFinished { get; private set; }

        public int GamesCount = 15;

        private PlayerStatsSync _playerStatsSync = new PlayerStatsSync();

        private List<Action> _gameActions = new List<Action>();

        private SyncListString _gameList = new SyncListString();
        private Queue<string> _gameQueue;

        private float _totalTime = 0;
        private int _playerFinishCount = 0;

        private string[] _games = new string[] { "PlayNail", "PlayScrew", "PlaySwipe" };

        private void Start()
        {
            _gameActions.Add(PlayScrew);
            _gameActions.Add(PlaySwipe);
            _gameActions.Add(PlayNail);

            if (isServer)
                StartCoroutine(WaitForPlayers());
        }

        /// <summary>
        /// Wait for all players to be connected without sending messages accross the network.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForPlayers()
        {
            if (!isServer)
                yield break;

            Debug.Log("Server ready, waiting for players... " + NetworkServer.connections.Count);

            while (!LobbyManager.Instance.AreAllClientsReady)
                yield return null;

            SetGameState(GAME_STATE.Play);
            GenerateGameList();
            RpcGameList();
            //StartCoroutine(ChoseGame());
        }

        /// <summary>
        /// Generates a set of games.
        /// </summary>
        /// <returns></returns>
        private SyncListString GenerateGameList()
        {
            _gameList.Clear();
            for (int i = 0; i < GamesCount; ++i)
            {
                _gameList.Add(_games[UnityEngine.Random.Range(0, _games.Length)]);
            }
            return _gameList;
        }

        [ClientRpc]
        private void RpcGameList()
        {
            Debug.Log("RPC game list size => " + _gameList.Count);
            _gameQueue = new Queue<string>(_gameList);
            GoToNextGame();
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
            Debug.Log("Client playing nail");
            CanvasManager.Instance.ChangeMiniGame(0);
        }

        private void PlaySwipe()
        {
            Debug.Log("Client playing swipe");
            CanvasManager.Instance.ChangeMiniGame(1);
        }

        [ClientRpc]
        private void RpcPlayScrew()
        {
            Debug.Log("Client playing screw");
            CanvasManager.Instance.ChangeMiniGame(2);
        }

        [Command]
        public void CmdSetChronoForPlayer(string playerName, float time)
        {
            var item = _playerStatsSync.FirstOrDefault(x => x.Name == playerName);
            if (item.IsEmpty())
            {
                item.Name = playerName;
                item.Time = 0;
                item.CurrentGameIdx = 1;
                _playerStatsSync.Add(item);
            }
            _totalTime += time;
            item.Time += time;
            Debug.Log("Registered total time for player " + playerName + " => " + _totalTime);
        }

        public void GoToNextGame()
        {
            if (_gameQueue.Count <= 0)
            {
                Debug.Log("All games ended !!");
                CanvasManager.Instance.DisplayWaitingRoom(_totalTime);
                CmdNotifyPlayerFinish();
                return;
            }
            // moves to the next game in the list
            // if no more, sends the time to the server
            Invoke(_gameQueue.Dequeue(), 0);
        }

        [Command]
        private void CmdNotifyPlayerFinish()
        {
            _playerFinishCount++;
            if (_playerFinishCount >= NetworkServer.connections.Count)
            {
                SetGameState(GAME_STATE.GameOver);
            }
        }
    }
}