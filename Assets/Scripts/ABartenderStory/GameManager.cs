using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Test
{
    public class GameManager : AGameManager
    {

        public Transform LeftTerrainLimit { get { return _leftSpawn; } }
        public Transform RightTerrainLimit { get { return _rightSpawn; } }

        [SerializeField] private GameObject _playerPrefab;

        [SerializeField] private Transform _leftSpawn;
        [SerializeField] private Transform _rightSpawn;

        [SerializeField] private Transform _leftLimit;
        [SerializeField] private Transform _rightLimit;

        [SerializeField] GameUI ui;

        private readonly List<GameObject> _players = new List<GameObject>();
        private int _finishedPlayer = 0;
        private PlayerController _winner = null;
        private PlayerController _looser = null;
        public int marge = 0;

        private string[] position =
        {
            "First",
            "Second",
            "third",
            "forth",
            "fith",
            "sixth",
            "seventh",
            "heighth"
        };

        private void Start()
        {
            if (isServer)
                StartCoroutine(SpawnPlayers());
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            LobbyManager.Instance.ShowLoadingScreen(false);
        }

        [Server]
        private void GameOver()
        {
            SetGameState(GAME_STATE.GameOver);
            Debug.Log("end of game !");
        }

        [Server]
        private IEnumerator SpawnPlayers()
        {
            if (!isServer)
                yield break;

            Debug.Log("Server ready, waiting for players... " + NetworkServer.connections.Count);

            while (!LobbyManager.Instance.AreAllClientsReady)
                yield return null;

            Debug.Log("Is server ? " + NetworkServer.active);
            var step = NetworkServer.connections.Count > 1 ? (_rightSpawn.position.x - _leftSpawn.position.x) / (NetworkServer.connections.Count - 1) : 0;
            var startSpawn = _leftSpawn.position;
            for (int i = 0; i < NetworkServer.connections.Count; ++i)
            {
                Debug.Log("Player created");
                _players.Add(Instantiate(_playerPrefab, startSpawn, Quaternion.identity));
                NetworkServer.AddPlayerForConnection(NetworkServer.connections[i], _players[i], (short)i);
                startSpawn.x += step;
            }
            SetGameState(GAME_STATE.Play);
        }

        [Server]
        private void FixedUpdate()
        {
            if (isServer)
            {
                if (GameState == GAME_STATE.Play)
                {
                    if (_finishedPlayer == _players.Count)
                    {
                        ui.RpcSetWinner(_winner._playerName + " won this game with " + Mathf.RoundToInt(_winner._score) + " points");
                        ui.RpcSetLooser(_looser._playerName + " lost this game with " + Mathf.RoundToInt(_looser._score) + " points, drink !!");
                        GameOver();
                    }
                }
                if (GameState == GAME_STATE.GameOver)
                    LooserDrunks();
            }
        }

        [Server]
        public void PlayerSetFinished(GameObject _player) {
            marge += 2;
            foreach(GameObject _object in _players) {
                if (_object == _player) {
                    _finishedPlayer++;
                    if (_winner == null) {
                        _winner = _player.GetComponent<PlayerController>();
                        _looser = _player.GetComponent<PlayerController>();
                    }
                    if (Mathf.RoundToInt(_player.GetComponent<PlayerController>()._score) > _winner._score) {
                        _winner = _player.GetComponent<PlayerController>();
                    } else if (Mathf.RoundToInt(_player.GetComponent<PlayerController>()._score) < _looser._score) {
                        _looser = _player.GetComponent<PlayerController>();
                    }
                }
            }
        }

        [Server]
        private void LooserDrunks()
        {
            for (int i = 0; i < _players.Count && _players[i].GetComponent<PlayerController>().LooserDrunk; i++)
            {
                if (i == (_players.Count) - 1)
                {
                    ChangeScene("GameSelectionScene");
                }
            }
        }
    }
}
