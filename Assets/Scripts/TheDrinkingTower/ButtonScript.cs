using Assets.Scripts.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Test {

    public class ButtonScript : NetworkBehaviour {
        [SyncVar] public bool isStriker = false;
        [SyncVar] public bool isAlive = true;
        [SyncVar] public bool asWon = false;
        public float rotationSpeed = 70f;
        BartenderGameManager gM = null;

        public GameObject bottle;
        public BottleScript bottleScript;

        public Text action1_text;
        public Text action2_text;

        public List<Image> timers = null;

        public float strikeDelay = 1f;
        public float fakeDelay = 0.5f;

        private float actionDelay = 0f;
        private float actualDelay = 0f;

        private GameObject _canvas = null;

        [SyncVar]
        public bool isDeleting = false;

        public GameObject mainCoaster = null;

        // Use this for initialization
        void Start() {
            if (isLocalPlayer) {
                _canvas = GameObject.Find("Canvas");
                RectTransform tmpRT = this.GetComponent<RectTransform>();
                tmpRT.anchoredPosition = Vector3.zero;
                tmpRT.anchorMin = new Vector2(0, 0);
                tmpRT.anchorMax = new Vector2(1, 1);
                tmpRT.pivot = new Vector2(0.5f, 0.5f);
                tmpRT.sizeDelta = _canvas.GetComponent<RectTransform>().rect.size;
                tmpRT.transform.SetParent(_canvas.GetComponent<RectTransform>());
                this.transform.SetParent(_canvas.transform);
                this.transform.localPosition = Vector3.zero;
            }
        }

        // Update is called once per frame
        void Update() {
            if (!isLocalPlayer) {
               this.gameObject.SetActive(false);
            }

            if (!isAlive && _canvas && this.bottleScript.coaster == null) {
                _canvas.GetComponent<CanvasScript>()._panel.SetActive(true);
                _canvas.GetComponent<CanvasScript>()._lost.SetActive(true);
            } else if (asWon && _canvas) {
                _canvas.GetComponent<CanvasScript>()._panel.SetActive(true);
                _canvas.GetComponent<CanvasScript>()._win.SetActive(true);
            } else if (isStriker && _canvas && !isAlive) {
                _canvas.GetComponent<CanvasScript>()._panel.SetActive(true);
                _canvas.GetComponent<CanvasScript>()._lost.SetActive(true);
            }

            gM = (AGameManager.Instance as BartenderGameManager);

            if (isStriker) {
                action1_text.text = "Strike";
                action2_text.text = "Fake";
            } else {
                action1_text.text = "Jump";
                action2_text.text = "Fall";
            }

            UpdateTimer();

            foreach (Image timer in timers) {
                if (actionDelay > 0)
                    timer.fillAmount = 1 / ((actualDelay / actionDelay));
                else
                    timer.fillAmount = 0;
            }
        }

        public void Action_1() {
            if (isLocalPlayer && actionDelay <= 0) {
                if (this.action1_text.text == "Strike") {
                    if (mainCoaster != null) {
                        CmdStartStrike();
                        actionDelay = strikeDelay;
                        actualDelay = actionDelay;
                    }
                } else {
                    CmdJump();
                    actionDelay = strikeDelay;
                    actualDelay = actionDelay;
                }
            }
        }

        public void Action_2() {
            if (isLocalPlayer && actionDelay <= 0) {
                if (this.action1_text.text == "Strike")
                    CmdStartFake();
                else
                    CmdFall();
                actionDelay = fakeDelay;
                actualDelay = actionDelay;
            }
        }

        [ClientRpc]
        public void RpcDelete() {
            this.isAlive = false;
            this.isDeleting = true;
        }

        [ClientRpc]
        public void RpcWin() {
            this.asWon = true;
        }

        public void UpdateTimer() {
            if (isLocalPlayer) {
                if (isDeleting) {
                    actionDelay = strikeDelay;
                    actualDelay = actionDelay;
                } else {
                    if (actionDelay > 0) {
                        actionDelay -= Time.deltaTime;
                    } else if (actionDelay != 0) {
                        actionDelay = 0;
                    }
                    if (isStriker) {
                        bottleScript.hammer = GameObject.Find("Hammer(Clone)");
                        bottleScript.hammer.transform.parent = bottle.transform;
                        CmdSetHammer();

                        if ((bottleScript.striking || bottleScript.faking) && (bottle.transform.eulerAngles.y == 0 || bottle.transform.eulerAngles.y > 180)) {
                            if (bottleScript.striking)
                                bottle.transform.Rotate(0, -2.5f * rotationSpeed * Time.deltaTime, 0);
                            else
                                bottle.transform.Rotate(0, -4 * rotationSpeed * Time.deltaTime, 0);
                        } else if (bottleScript.striking || bottleScript.faking) {
                            bottle.transform.localEulerAngles = Vector3.zero;
                            if (bottleScript.striking) {
                                CmdStrike();
                                bottleScript.striking = false;
                            } else {
                                CmdFake();
                                bottleScript.faking = false;
                            }
                        }
                    }
                }
            }
        }

        [Command]
        public void CmdSetHammer() {
            bottleScript.hammer = GameObject.Find("Hammer(Clone)");
            bottleScript.hammer.transform.parent = bottle.transform;
        }

        [Command]
        public void CmdStartStrike() {
            if (isServer) {
                this.bottleScript.striking = true;
            }
        }

        [Command]
        public void CmdStrike() {
            if (isServer) {
                if (mainCoaster) {
                    mainCoaster.GetComponent<CoasterScript>().SetStriked(true);
                    gM.Strike();
                }
            }
        }

        [Command]
        public void CmdStartFake() {
            if (isServer) {
                this.bottleScript.faking = true;
            }
        }

        [Command]
        public void CmdFake() {
        }

        [Command]
        public void CmdJump() {
            if (isServer) {
                bottleScript.CmdJumping(true);
            }
        }


        [Command]
        public void CmdFall() {
        }

        [ClientRpc]
        public void RpcSetBottle(string name) {
            bottle = GameObject.Find(name);
            this.bottleScript = bottle.GetComponent<BottleScript>();
        }
    }
}
