using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.CantRoachThis
{
    /// <summary>
    /// Game manager for the cockroach game.
    /// </summary>
    public class GameManager : AGameManager
    {
        public Transform LeftTerrainLimit { get { return _leftSpawn; } }
        public Transform RightTerrainLimit { get { return _rightSpawn; } }

        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _swatterPrefab;

        [SerializeField] private Transform _leftSpawn;
        [SerializeField] private Transform _rightSpawn;

        [SerializeField] private int _maxSwatters = 3;
        [SerializeField] private float _swatterSpawnDelay = 5f;

        private float _currentDelay = 3;

        private readonly List<GameObject> _players = new List<GameObject>();
        private readonly SyncListString _playersDead = new SyncListString();
        private readonly List<GameObject> _swatters = new List<GameObject>();

        /// <summary>
        /// The list of dead players.
        /// </summary>
        public SyncListString PlayersDead { get { return _playersDead; } }

        private void Start()
        {
            if (isServer)
                StartCoroutine(SpawnObjects());
        }

        private void Update()
        {
            if (isServer && GameState == GAME_STATE.Play)
            {
                _currentDelay -= Time.deltaTime;
                if (_currentDelay < 0)
                {
                    _currentDelay = _swatterSpawnDelay;
                    var s = _swatters[0];
                    s.GetComponent<SwatterController>().Activate(true);
                    _swatters.RemoveAt(0);
                    _swatters.Add(s);
                }
            }
        }

        /// <summary>
        /// Kills a player on the server.
        /// </summary>
        /// <param name="controller"></param>
        public void KillPlayer(APlayerController controller)
        {
            if (isServer)
            {
                if (controller == null || !_players.Remove(controller.gameObject))
                {
                    Debug.LogError("Object not found in the player list, or is null.");
                    return;
                }
                NetworkServer.Destroy(controller.gameObject);
                _playersDead.Insert(0, controller._playerName);
                if (_players.Count <= 1)
                {
                    foreach (var player in _players)
                        _playersDead.Insert(0, player.GetComponent<APlayerController>()._playerName);
                    GameOver();
                }
            }
        }

        /// <summary>
        /// Sets the game over.
        /// </summary>
        [Server]
        private void GameOver()
        {
            SetGameState(GAME_STATE.GameOver);
            Debug.Log("end of game !");
        }

        /// <summary>
        /// Spawns all the objects needed for the game.
        /// </summary>
        /// <returns></returns>
        [Server]
        private IEnumerator SpawnObjects()
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
                _players.Add(Instantiate(_playerPrefab, startSpawn, Quaternion.identity));
                NetworkServer.AddPlayerForConnection(NetworkServer.connections[i], _players[i], (short)i);
                startSpawn.x += step;
            }
            for (int i = 0; i < _maxSwatters; ++i)
            {
                var _swatter = Instantiate(_swatterPrefab);
                _swatters.Add(_swatter);
                NetworkServer.Spawn(_swatter);
            }
            SetGameState(GAME_STATE.Play);
        }
    }
}