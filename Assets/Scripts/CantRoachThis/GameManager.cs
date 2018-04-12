using Assets.Scripts.Networking;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.CantRoachThis
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;

        private GameObject[] _players;

        private void Start()
        {
            StartCoroutine(SpawnPlayers());
        }

        [Server]
        private IEnumerator SpawnPlayers()
        {
            if (!isServer)
                yield break;

            while (!LobbyManager.Instance.AreAllClientsReady)
                yield return null;

            Debug.Log("Is server ? " + NetworkServer.active);
            _players = new GameObject[LobbyManager.Instance.numPlayers];
            for (int i = 0; i < LobbyManager.Instance.numPlayers; ++i)
            {
                _players[i] = Instantiate(_playerPrefab);
                NetworkServer.AddPlayerForConnection(NetworkServer.connections[i], _players[i], (short)i);
            }
        }
    }
}