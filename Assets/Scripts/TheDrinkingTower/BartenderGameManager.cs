using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Assets.Scripts.Test {

    public class BartenderGameManager : AGameManager {

        [SerializeField] public List<GameObject> coasters = null;
        [SerializeField] private List<Color> possibleColors = null;

        [SerializeField] private GameObject _hammerPrefab = null;
        [SerializeField] private GameObject _strikerPrefab = null;
        [SerializeField] private GameObject _bottlePrefab = null;
        [SerializeField] private GameObject _coasterPrefab = null;

        [SerializeField] private float marge_x = 1f;
        [SerializeField] private float marge_z = 0f;

        private GameObject[] _players;
        private List<GameObject> _losers = new List<GameObject>();
        private GameObject[] _bottles = null;
        private GameObject _hammer = null;

        private bool mainCoasterUpdated = true;

        public bool gameStarted = false;
        public bool gameIsFinished = false;

        // Use this for initialization
        [Server]
        void Start() {
            StartCoroutine(SpawnPlayers());
        }

        [Server]
        private IEnumerator SpawnPlayers() {

            if (!isServer)
                yield break;

            Debug.Log("Server ready, waiting for players... " + NetworkServer.connections.Count);

            while (!LobbyManager.Instance.AreAllClientsReady)
                yield return null;

//            Debug.Log("Is server ? " + NetworkServer.active);

            for (int i = 0; i < 6; i++) {
                coasters.Add(Instantiate(_coasterPrefab, new Vector3(4, 1.6f - i * 0.6f, 1), Quaternion.identity));
                NetworkServer.Spawn(coasters[i]);
                coasters[i].GetComponent<CoasterScript>().RpcName("Coaster " + i);
            }

            _players = new GameObject[LobbyManager.Instance.numPlayers];
            _bottles = new GameObject[LobbyManager.Instance.numPlayers];

            for (int i = 0; i < NetworkServer.connections.Count; i++) {

                _players[i] = Instantiate(_strikerPrefab, Vector3.zero, Quaternion.identity);
                if (i == 0) {
                    _bottles[i] = Instantiate(_bottlePrefab, new Vector3(-1.5f, 0, 0), Quaternion.identity);
                } else {
                    Vector3 bottlePosition = coasters[0].transform.position;

                    bottlePosition.y += 1.3f;
                    bottlePosition.z += marge_z;
                    bottlePosition.x += marge_x - 2.5f;
                    marge_x += 1.5f;
                    if (marge_x >= 4) {
                        marge_x = 1.5f;
                        marge_z = 1f;
                    }

                    _bottles[i] = Instantiate(_bottlePrefab, bottlePosition, Quaternion.identity);
                }
                if (i == 0) {
                    _players[i].GetComponent<ButtonScript>().isStriker = true;
                } else {
                    _players[i].GetComponent<ButtonScript>().isStriker = false;
                }

                _players[i].GetComponent<ButtonScript>().mainCoaster = coasters[0];
                _players[i].GetComponent<ButtonScript>().bottle = _bottles[i];
                _players[i].GetComponent<ButtonScript>().bottleScript = _bottles[i].GetComponent<BottleScript>();
                NetworkServer.AddPlayerForConnection(NetworkServer.connections[i], _players[i], (short)i);
                NetworkServer.SpawnWithClientAuthority(_bottles[i], _players[i]);
            }
            _hammer = Instantiate(_hammerPrefab, _players[0].transform.position, Quaternion.identity);
            NetworkServer.SpawnWithClientAuthority(_hammer, _players[0]);
            gameStarted = true;
            Debug.Log("GameStarted");
        }

        [Server]
        public void Strike() {
            if (coasters.Count > 0) {
                coasters.Remove(coasters[0]);
                int i = 0;
                foreach (GameObject player in _players) {
                    ButtonScript playerScript = player.GetComponent<ButtonScript>();
                    if (playerScript.isAlive && (playerScript.bottle.GetComponent<BottleScript>().jumping || playerScript.bottle.GetComponent<BottleScript>().falling || playerScript.isStriker)) {
                        if (coasters.Count > 0) {
                            player.GetComponent<ButtonScript>().mainCoaster = coasters[0];
                        } else {
                            player.GetComponent<ButtonScript>().mainCoaster = null;
                        }
                    } else if (playerScript.isAlive) {
                        RemovePlayer(player);
                        player.GetComponent<ButtonScript>().RpcDelete();
                    }
                    i++;
                }
            }
            mainCoasterUpdated = true;
        }

        [Server]
        public void RemovePlayer(GameObject player) {
            _losers.Add(player);
        }

        bool tmpCheck = false;

        bool SceneLoaded = false;
        // Update is called once per frame
        [Server]
        void Update() {
            if (gameStarted) {
//                Debug.Log(_losers.Count + " " + NetworkServer.connections.Count);
                if (coasters.Count == 0 || _losers.Count == NetworkServer.connections.Count - 1) {
                    gameIsFinished = true;
                }

                if (gameIsFinished) {
                    for (int i = 0; i < _players.Length && _players[i].GetComponent<ButtonScript>().LooserDrunk; i++) {
                        if (i == (_players.Length) - 1 && !SceneLoaded) {
                            ChangeScene("GameSelectionScene");
                            SceneLoaded = true;
                        }
                    }
                    if (tmpCheck == true)
                        return;
                    tmpCheck = true;
                    if (_losers.Count >= NetworkServer.connections.Count - 1) {
                        _players[0].GetComponent<ButtonScript>().RpcWin();
                    } else {
                        _players[0].GetComponent<ButtonScript>().RpcDelete();
                        foreach (GameObject player in _players) {
                            if (player.GetComponent<ButtonScript>().isAlive && !player.GetComponent<ButtonScript>().isStriker)
                                player.GetComponent<ButtonScript>().RpcWin();
                        }
                    }
                }
                if (possibleColors.Count > 0 && coasters.Count > 0) {
                    foreach (GameObject coaster in coasters) {
                        coaster.GetComponent<CoasterScript>().ObjectColor = Random.Range(0, possibleColors.Count);
                    }
                    coasters[0].GetComponent<CoasterScript>().mainCoaster = true;
                    possibleColors.Clear();
                }
                if (mainCoasterUpdated) {
                    mainCoasterUpdated = false;
                    marge_x = 1f;
                    marge_z = 0f;
                    int i = 0;
                    foreach (GameObject bottle in _bottles) {
                        if (_players[i].GetComponent<ButtonScript>().isAlive) {
                            if (i > 0) {
                                bottle.GetComponent<BottleScript>().RpcCoaster(_players[i].GetComponent<ButtonScript>().mainCoaster);
                                bottle.GetComponent<BottleScript>().RpcName("Jumper " + i);
                                bottle.GetComponent<BottleScript>().RpcMarge(i * 2);
                                _players[i].GetComponent<ButtonScript>().RpcSetBottle("Jumper " + i);

                            } else {
                                bottle.GetComponent<BottleScript>().isStriker = true;
                                bottle.GetComponent<BottleScript>().RpcName("Striker " + i);
                                _players[i].GetComponent<ButtonScript>().RpcSetBottle("Striker " + i);
                            }
                        }
                        i++;
                    }
                }
            }
        }
    }
}
