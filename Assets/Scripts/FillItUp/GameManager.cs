using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.FillItUp
{
    public class GameManager : AGameManager
    {

        public Transform LeftTerrainLimit { get { return _leftSpawn; } }
        public Transform RightTerrainLimit { get { return _rightSpawn; } }

        [SerializeField] private float GameDuration = 60.0f;

        [SerializeField] private GameObject _playerPrefab;

        [SerializeField] private Transform _leftSpawn;
        [SerializeField] private Transform _rightSpawn;

        [SerializeField] private Transform _leftLimit;
        [SerializeField] private Transform _rightLimit;

        [SerializeField] Assets.Scripts.GoSoju.GameUI ui;

        private readonly List<GameObject> _players = new List<GameObject>();
        [SerializeField] private EmiterManager emiterManager;

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
                _players.Add(Instantiate(_playerPrefab, startSpawn, Quaternion.identity));
                NetworkServer.AddPlayerForConnection(NetworkServer.connections[i], _players[i], (short)i);
                startSpawn.x += step;
            }
            SetGameState(GAME_STATE.Play);
            emiterManager.RpcSeed(Random.Range(0, 1000000000));
            emiterManager.RpcStartEmision();
        }

        [Server]
        private void FixedUpdate()
        {
            if (isServer)
            {
                if (GameState == GAME_STATE.Play)
                {
                    GameDuration -= Time.deltaTime;
                    if (GameDuration < 0)
                    {
                        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
                        for (int i = 0; i < player.Length -1; i++)
                        {
                            if (player[i].GetComponent<PlayerController>().height <
                                player[i + 1].GetComponent<PlayerController>().height)
                            {
                                GameObject tmp = player[i];
                                player[i] = player[i + 1];
                                player[i + 1] = tmp;
                                i = 0;
                            }
                        }
                        ui.RpcSetWinner(player[0].GetComponent<PlayerController>()._playerName + " won this game");
                        ui.RpcSetLooser(player[player.Length - 1].GetComponent<PlayerController>()._playerName + " lost this game, drink !!");
                        emiterManager.RpcStopEmission();
                        GameOver();
                    }
                }
                if (GameState == GAME_STATE.GameOver)
                    LooserDrunks();
            }
        }

        private bool sceneLoaded = false;
        [Server]
        private void LooserDrunks()
        {
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < player.Length && player[i].GetComponent<PlayerController>().LooserDrunk; i++)
            {
                if (i == (player.Length - 1) && !sceneLoaded)
                {
                    ChangeScene("GameSelectionScene");
                    sceneLoaded = true;
                }
            }
        }
    }
}
