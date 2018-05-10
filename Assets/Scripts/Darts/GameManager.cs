using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Darts
{
    public class GameManager : AGameManager
    {
        [SerializeField] private GameObject _playerPrefab;

        private readonly List<GameObject> _players = new List<GameObject>();
        private readonly List<Result> _results = new List<Result>();

        private void Start()
        {
            if (isServer)
                StartCoroutine(SpawnObjects());
        }

        public void SetScore(string playerName, float score)
        {
            _results.Add(new Result { PlayerName = playerName, Score = score });
            Debug.LogError(playerName + " " + score);
            if (_results.Count >= LobbyManager.Instance.ConnectionCount)
            {
                SetGameState(GAME_STATE.GameOver);
            }
        }

        [Server]
        private IEnumerator SpawnObjects()
        {
            if (!isServer)
                yield break;

            Debug.Log("Server ready, waiting for players... " + NetworkServer.connections.Count);

            while (!LobbyManager.Instance.AreAllClientsReady)
                yield return null;

            for (int i = 0; i < NetworkServer.connections.Count; ++i)
            {
                _players.Add(Instantiate(_playerPrefab, Vector3.zero, Quaternion.identity));
                NetworkServer.AddPlayerForConnection(NetworkServer.connections[i], _players[i], (short)i);
            }

            Debug.Log("Is server ? " + NetworkServer.active);
            SetGameState(GAME_STATE.Play);
        }
    }

    public class Result
    {
        public string PlayerName;
        public float Score;
    }
}