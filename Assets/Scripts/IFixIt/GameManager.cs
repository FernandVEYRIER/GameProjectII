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

        public PlayerStatsSync PlayerStatsList {  get { return _playerStatsSync; } }

        public int GamesCount = 15;

        private PlayerStatsSync _playerStatsSync = new PlayerStatsSync();

        private List<Action> _gameActions = new List<Action>();

        private SyncListString _gameList = new SyncListString();
        private Queue<string> _gameQueue;

        private float _totalTime = 0;
        private int _playerFinishCount = 0;

        private string[] _games = new string[] { "PlayNail", "PlayScrew", "PlaySwipe" };

        private Animator _animator;
        private bool _move = true;

        private void Start()
        {
            _animator = Camera.main.GetComponent<Animator>();

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

            SetGameState(GAME_STATE.WarmUp);
            GenerateGameList();
            yield return new WaitForSeconds(3);
            SetGameState(GAME_STATE.Play);
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
            _move = true;
            Debug.Log("Playing screw game");
            //AllPlayersFinished = false;
            //CanvasManager.Instance.ChangeMiniGame(2);
            _animator.SetFloat("Blend", 2);
            _animator.SetBool("Move", _move);
        }

        private void PlayNail()
        {
            _move = true;
            Debug.Log("Client playing nail");
            //CanvasManager.Instance.ChangeMiniGame(0);
            _animator.SetFloat("Blend", 0);
            _animator.SetBool("Move", _move);
        }

        private void PlaySwipe()
        {
            _move = true;
            Debug.Log("Client playing swipe");
            //CanvasManager.Instance.ChangeMiniGame(1);
            _animator.SetFloat("Blend", 1);
            _animator.SetBool("Move", _move);
        }

        //[ClientRpc]
        //private void RpcPlayScrew()
        //{
        //    Debug.Log("Client playing screw");
        //    CanvasManager.Instance.ChangeMiniGame(2);
        //    _animator.SetFloat("Blend", 2);
        //    _animator.SetBool("Move", true);
        //}

        [Command]
        public void CmdSetChronoForPlayer(string playerName, float time)
        {
            PlayerStats item = new PlayerStats();
            var idx = 0;
            for (int i = 0; i < _playerStatsSync.Count; ++i)
            {
                if (_playerStatsSync[i].Name == playerName)
                {
                    item = _playerStatsSync[i];
                    idx = i;
                }
            }
            if (item.IsEmpty())
            {
                item.Name = playerName;
                item.Time = 0;
                item.CurrentGameIdx = 1;
                _playerStatsSync.Add(item);
            }
            _totalTime += time;
            item.Time += time;
            Debug.Log("idx of => " + idx);
            _playerStatsSync[idx] = item;
            Debug.Log("Registered total time for player " + playerName + " => " + _totalTime);
        }

        public void GoToNextGame()
        {
            _move = false;
            _animator.SetBool("Move", _move);
            if (_gameQueue.Count <= 0)
            {
                Debug.Log("All games ended !!");
                CanvasManager.Instance.DisplayWaitingRoom(_totalTime);
                CmdNotifyPlayerFinish();
                return;
            }

            StartCoroutine(GoToNextGameCoroutine());
        }

        private IEnumerator GoToNextGameCoroutine()
        {
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).IsTag("Start"));
            // moves to the next game in the list
            // if no more, sends the time to the server
            Debug.Log("Animator info => " + _animator.GetCurrentAnimatorStateInfo(0).IsTag("Start"));
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

        public void ChangeMiniGame(uint i)
        {
            if (_move)
                CanvasManager.Instance.ChangeMiniGame(i);
        }
    }
}