using System;
using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


namespace Assets.Scripts.Test
{
    public class PlayerController : APlayerControllerEnzo
    {
        [SerializeField] private float speed = 1;

        private NetworkInstanceId _networkIdentity;

        public bool stopMoving;

        private GoSoju.GameUI ui;
        private GameObject target = null;
        public Light _light = null;

        private Vector3 originalPosition = Vector3.zero;
        private Vector3 oldMousePosition = Vector3.zero;
        private float pushForce = 0;
        private bool canMove = true;
        public float _score = 0;
        private int round = 0;
        private bool scoreDisplayed = true;

        /// <summary>
        /// Called when the local player is ready.
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            SetupPlayer();
            Debug.Log("Starting local player " + _playerColor + "   " + _playerName);
            looserDrunk = false;
        }

        /// <summary>
        /// Sets up the player values.
        /// </summary>
        [Client]
        private void SetupPlayer()
        {
            _networkIdentity = GetComponent<NetworkIdentity>().netId;

            ui = GameObject.Find("PlayerUI").GetComponent<GoSoju.GameUI>();
            target = GameObject.Find("Bar/Cible/Cylinder");
            name = _playerName;
            ui.SetPlayer(this);
            _light.color = _playerColor;
            CmdSetPlayerLightColor();
        }

        [Command]
        override protected void CmdSetPlayerInfo(PlayerInfo info)
        {
            base.CmdSetPlayerInfo(info);
            print("color of player " + _playerColor);
        }

        [Command]
        public void CmdSetPlayerLightColor() {
            _light.color = _playerColor;
        }

        private void Update()
        {
            _light.color = _playerColor;
            if (isLocalPlayer)
            {
                if (ui  != null) {
                    if (ui.GameStart) {
                        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && canMove == true) {
                            // Get movement of the finger since last frame
                            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

                            if (touchDeltaPosition.x < 0) {
                                pushForce = -touchDeltaPosition.x;
                                canMove = false;
                            }
                        } else if (Input.GetMouseButtonDown(0) && canMove == true) {
                            originalPosition = this.transform.position;
                            oldMousePosition = Input.mousePosition;
                        } else if (Input.GetMouseButtonUp(0) && Input.mousePosition.x - oldMousePosition.x > 0 && oldMousePosition != Vector3.zero) {
                            pushForce = (Input.mousePosition.x - oldMousePosition.x) / 10;
                            oldMousePosition = Vector3.zero;
                            canMove = false;
                        } else if (Input.GetMouseButtonUp(0) && Input.mousePosition.x - oldMousePosition.x < 0) {
                            oldMousePosition = Vector3.zero;
                        }
                        transform.Translate(transform.right * speed * pushForce * Time.deltaTime);
                        if (pushForce > 0 && transform.position.x <= 38) {
                            pushForce -= Time.deltaTime;
                            scoreDisplayed = false;
                        } else if (pushForce > 0 && transform.position.x > 38) {
                            pushForce = 0;
                            scoreDisplayed = false;
                        } else if (pushForce <= 0 && canMove == false && scoreDisplayed == false) {
                            scoreDisplayed = true;
                            pushForce = 0;
                            _score += 1;
                            if (transform.position.x <= 38) {
                                _score += (100 - (Mathf.Abs(this.transform.position.x - target.transform.position.x) * 100 / Mathf.Abs(originalPosition.x - target.transform.position.x)));
                            }
                            this.transform.position = originalPosition;
                            if (++round < 3) {
                                canMove = true;
                                GameObject.Find("PlayerUI/Round").GetComponent<Text>().text = (round + 1).ToString();
                            } else {
                                GameObject.Find("PlayerUI/Round").GetComponent<Text>().text = "finished";
                                CmdFinish(_score, originalPosition);
                            }
                            GameObject.Find("PlayerUI/Score").GetComponent<Text>().text = _score.ToString("f0");
                        }
                    }
                } else {
                    ui = GameObject.Find("PlayerUI").GetComponent<GoSoju.GameUI>();
                    ui.SetPlayer(this);
                    target = GameObject.Find("Bar/Cible/Cylinder");
                    name = _playerName;
                }
            }
        }

        [Command]
        private void CmdFinish(float score, Vector3 _position) {
            _score = score;
            GameManager gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            gm.PlayerSetFinished(this.gameObject);
            _position.y += 4f;
            _position.x -= gm.marge;
            this.transform.position = _position;
            RpcFinish(_position);
        }

        [ClientRpc]
        private void RpcFinish(Vector3 _position) {
            this.transform.position = _position;
        }

        protected override void OnPlayerColorChange(Color color) {
            _playerColor = color;
            _light.color = color;
        }
    }
}
