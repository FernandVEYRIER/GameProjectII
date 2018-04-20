using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.GoSoju
{ 
    public class GameManager : AGameManager
    {

        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private GameObject _glass;

        [SerializeField] private Transform _leftSpawn;
        [SerializeField] private Transform _rightSpawn;
        [SerializeField] private Transform _bottleSpawn;
        [SerializeField] private Transform _glassSpawn;


        private GameObject[] _players;
        private GameObject[] _glassToFile;
        private string[] winnerOrder;

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

        [Server]
        private void Start()
        {
            StartCoroutine(SpawnPlayers());
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
            _glassToFile = new GameObject[LobbyManager.Instance.numPlayers];
            winnerOrder = new string[LobbyManager.Instance.numPlayers];
            var step = NetworkServer.connections.Count > 1 ? (_rightSpawn.position.z - _leftSpawn.position.z) / (NetworkServer.connections.Count - 1) : 0;
            var startSpawn = _leftSpawn.position;
            print("step = " + step);
            for (int i = 0; i < NetworkServer.connections.Count; ++i)
            {
                startSpawn.x = _bottleSpawn.position.x;
                startSpawn.y = _bottleSpawn.position.y;
                _players[i] = Instantiate(_playerPrefab, startSpawn, Quaternion.identity);
                NetworkServer.AddPlayerForConnection(NetworkServer.connections[i], _players[i], (short)i);
                print("playeur position = " + _players[i].transform.position);
                startSpawn.x = _glassSpawn.position.x;
                startSpawn.y = _glassSpawn.position.y;
                _glassToFile[i] = Instantiate(_glass, startSpawn, Quaternion.identity);
                NetworkServer.Spawn(_glassToFile[i]);
                print("glass position = " + _glassToFile[i].transform.position);
                startSpawn.z += step;
                
            }
        }

        [Server]
        public void playerArrived(string name, PlayerController player)
        {
            for (int i = 0; i < LobbyManager.Instance.numPlayers; ++i)
            {
                if (winnerOrder[i] == null)
                {
                    winnerOrder[i] = name;
                    print(name + "is " + i);
                    player.position = position[i];
                    return ;
                }
            }
        }
    }
}
