using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.CantRoachThis
{
    /// <summary>
    /// Game manager for the cockroach game.
    /// </summary>
    public class GameManager : AGameManager
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _swatterPrefab;

        [SerializeField] private Transform _leftSpawn;
        [SerializeField] private Transform _rightSpawn;

        private GameObject[] _players;
        private GameObject _swatter;

        private void Start()
        {
            StartCoroutine(SpawnPlayers());
        }

        /// <summary>
        /// Kills a player on the server.
        /// </summary>
        /// <param name="gameObject"></param>
        public void KillPlayer(GameObject gameObject)
        {
            if (isServer)
            {
                NetworkServer.Destroy(gameObject);
            }
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
            _players = new GameObject[LobbyManager.Instance.numPlayers];
            var step = NetworkServer.connections.Count > 1 ? (_rightSpawn.position.x - _leftSpawn.position.x) / (NetworkServer.connections.Count - 1) : 0;
            var startSpawn = _leftSpawn.position;
            for (int i = 0; i < NetworkServer.connections.Count; ++i)
            {
                _players[i] = Instantiate(_playerPrefab, startSpawn, Quaternion.identity);
                NetworkServer.AddPlayerForConnection(NetworkServer.connections[i], _players[i], (short)i);
                startSpawn.x += step;
            }
            _swatter = Instantiate(_swatterPrefab);
            NetworkServer.Spawn(_swatter);
        }
    }
}