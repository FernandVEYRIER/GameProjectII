using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.GoSoju
{ 
    public class GameManager : AGameManager
    {

        [SerializeField] private GameObject _strikerPrefab;
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

        private void Start()
        {
            if (isServer)
                StartCoroutine(SpawnPlayers());
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            LobbyManager.Instance.panelLoading.gameObject.SetActive(false);
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
            _players = new GameObject[LobbyManager.Instance.numPlayers];
            _glassToFile = new GameObject[LobbyManager.Instance.numPlayers];
            winnerOrder = new string[LobbyManager.Instance.numPlayers];
            var startSpawn = _leftSpawn.position;

            for (int i = 0; i < NetworkServer.connections.Count; ++i)
            {
                if (i == 0) {
                    _players[i] = Instantiate(_strikerPrefab, startSpawn, Quaternion.identity);
                } else {
                    _players[i] = Instantiate(_playerPrefab, startSpawn, Quaternion.identity);
                }

                NetworkServer.AddPlayerForConnection(NetworkServer.connections[i], _players[i], (short)i);
                print("playeur position = " + _players[i].transform.position);
                startSpawn.x = _glassSpawn.position.x;
                startSpawn.y = _glassSpawn.position.y;
                _glassToFile[i] = Instantiate(_glass, startSpawn, Quaternion.identity);
                NetworkServer.Spawn(_glassToFile[i]);
                print("glass position = " + _glassToFile[i].transform.position);
                startSpawn.z += step;
                
            }
            SetGameState(GAME_STATE.Play);
        }

        [Server]
        public void playerArrived(string name, PlayerController player)
        {
            int playerNbr;
            for (playerNbr = 0; playerNbr < LobbyManager.Instance.numPlayers; ++playerNbr)
            {
                if (winnerOrder[playerNbr] == null)
                {
                    winnerOrder[playerNbr] = name;
                    print(name + "is " + playerNbr);
                    player.position = position[playerNbr];
                    player.finish = true;
                    break;
                }
            }
            /*if (playerNbr + 1 >= LobbyManager.Instance.numPlayers - 1)
                GameOver();*/
        }

        [Server]
        private void FixedUpdate()
        {
            if (GameState == GAME_STATE.Play)
            {
                SortPlayer();
                GameFinished();
            }
            if (GameState == GAME_STATE.GameOver)
                LooserDrunks();
        }

        // TODO : fix this omg
        private bool loading;
        [Server]
        private void LooserDrunks()
        {
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < player.Length && player[i].GetComponent<PlayerController>().LooserDrunk; i++)
            {
                if (i == (player.Length - 1) && !loading)
                {
                    ChangeScene("GameSelectionScene");
                    loading = true;
                }
            }
        }

        [Server]
        private void GameFinished()
        {
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < player.Length && player[i].GetComponent<PlayerController>().finish == true; i++)
            {

                if (i == (player.Length - 1))
                {
                    foreach (string s in winnerOrder)
                        print(s);
                    ui.RpcSetWinner(winnerOrder[0] + " won this game");
                    ui.RpcSetLooser(winnerOrder[i] + " lost this game, drink !!");
                    GameOver();
                }
            }
        }

        [Server]
        private void SortPlayer()
        {
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            GameObject tmp;
            for (int i = 0; i < player.Length - 1; i++)
            {
                if (player[i].transform.position.x < player[i + 1].transform.position.x)
                {
                    tmp = player[i + 1];
                    player[i + 1] = player[i];
                    player[i] = tmp;
                    i = 0;
                }
            }
            for (int i = 0; i < player.Length; i++)
            {
                player[i].GetComponent<PlayerController>().position = position[i];
                player[i].GetComponent<PlayerController>().nbrPosition = (i + 1).ToString();
            }
        }
    }
}

        [SerializeField] private GameObject _glassPrefab;
        [SerializeField] private Transform _glassSpawn;
        [SerializeField] GameUI ui;
        private GameObject[] _glass;
            _glass = new GameObject[LobbyManager.Instance.numPlayers];
                startSpawn.x = _glassSpawn.position.x;
                startSpawn.y = _glassSpawn.position.y;
                _glass[i] = Instantiate(_glassPrefab, startSpawn, Quaternion.identity);
                NetworkServer.Spawn(_glass[i]);
                print("glass position = " + _glass[i].transform.position);
                startSpawn.z += step;