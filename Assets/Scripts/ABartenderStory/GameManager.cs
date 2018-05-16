using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Assets.Scripts.ABartenderStory {

    public class GameManager : AGameManager {

        [SerializeField] private GameObject canva = null;
        [SerializeField] public List<GameObject> coasters = null;
        [SerializeField] private List<Color> possibleColors = null;

        [SerializeField] private GameObject _hammerPrefab;
        [SerializeField] private GameObject _strikerPrefab;
        [SerializeField] private GameObject _bottlePrefab;
        [SerializeField] private GameObject _coasterPrefab;

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
            }


            _players = new GameObject[LobbyManager.Instance.numPlayers];
            _bottles = new GameObject[LobbyManager.Instance.numPlayers];
            

            GameObject tmpCanvas = Instantiate(canva, Vector3.zero, Quaternion.identity);
            NetworkServer.Spawn(tmpCanvas);

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
                    if (player.GetComponent<ButtonScript>().bottle.GetComponent<BottleScript>().jumping || player.GetComponent<ButtonScript>().bottle.GetComponent<BottleScript>().falling || player.GetComponent<ButtonScript>().isStriker) {
                        if (coasters.Count > 0) {
                            player.GetComponent<ButtonScript>().mainCoaster = coasters[0];
                        } else {
                            player.GetComponent<ButtonScript>().mainCoaster = null;
                        }
                    } else {
                        _bottles[i] = null;
                        RemovePlayer(player);
                        player.GetComponent<ButtonScript>().RpcDelete();
                    }
                    i++;
                }
                mainCoasterUpdated = true;
            }
        }

        [Server]
        public void RemovePlayer(GameObject player) {
            _losers.Add(player);
        }

        bool tmpCheck = false;

        // Update is called once per frame
        [Server]
        void Update() {
            if (gameStarted) {
                if (coasters.Count == 0 || _losers.Count == LobbyManager.Instance.numPlayers - 1) {
                    gameIsFinished = true;
                }

                if (gameIsFinished) {
                    if (tmpCheck == true)
                        return;
                    tmpCheck = true;
                    if (_losers.Count >= LobbyManager.Instance.numPlayers - 1) {
                        Debug.Log("Jumpers loose");
                    } else {
                        Debug.Log("Striker loose");
                    }
                }
                if (possibleColors.Count > 0 && coasters.Count > 0) {
                    foreach (GameObject coaster in coasters) {
                        coaster.GetComponent<CoasterScript>().ObjectColor = Random.Range(0, possibleColors.Count);
                    }
                    coasters[0].GetComponent<CoasterScript>().mainCoaster = true;
                    possibleColors.Clear();
                }
                if (coasters.Count > 0 && mainCoasterUpdated) {
                    mainCoasterUpdated = false;
                    marge_x = 1f;
                    marge_z = 0f;
                    int i = 0;
                    foreach (GameObject bottle in _bottles) {
                        if (bottle) {
                            if (i > 0) {
                                Vector3 bottlePosition = _players[i].GetComponent<ButtonScript>().mainCoaster.transform.position;
                                bottle.GetComponent<BottleScript>().RpcCoaster(_players[i].GetComponent<ButtonScript>().mainCoaster);
                                bottle.GetComponent<BottleScript>().RpcName("Jumper " + i);

                                bottlePosition.z += marge_z;
                                bottlePosition.y += 1.3f;
                                bottlePosition.x += marge_x - 2.5f;
                                marge_x += 1.5f;
                                if (marge_x >= 4f) {
                                    marge_x = 1.5f;
                                    marge_z = 1f;
                                }
                                bottle.GetComponent<BottleScript>().transform.position = bottlePosition;
                            } else {
                                bottle.GetComponent<BottleScript>().isStriker = true;
                                bottle.GetComponent<BottleScript>().RpcName("Striker " + i);
                            }
                            i++;
                        }
                    }
                }
            }
        }
    }
}
