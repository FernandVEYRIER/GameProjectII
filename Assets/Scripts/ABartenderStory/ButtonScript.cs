﻿using Assets.Scripts.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.ABartenderStory {

    public class ButtonScript : NetworkBehaviour {
        [SyncVar]
        public bool isStriker;
        BartenderGameManager gM = null;

        public GameObject bottle;

        public Text action1_text;
        public Text action2_text;

        public List<Image> timers = null;

        public float strikeDelay = 1f;
        public float fakeDelay = 0.5f;

        private float actionDelay = 0f;
        private float actualDelay = 0f;

        [SyncVar]
        public bool isDeleting = false;

        public GameObject mainCoaster = null;

        // Use this for initialization
        void Start() {
            if (isLocalPlayer) {
                this.transform.SetParent(GameObject.Find("Canvas(Clone)").transform);
                this.transform.localPosition = Vector3.zero;
            }
        }

        // Update is called once per frame
        void Update() {
            if (!isLocalPlayer) {
                this.gameObject.SetActive(false);
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
                        CmdStrike();
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
                    CmdFake();
                else
                    CmdFall();
                actionDelay = fakeDelay;
                actualDelay = actionDelay;
            }
        }

        [ClientRpc]
        public void RpcDelete() {
            this.isDeleting = true;
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
                }
            }
        }

        [Command]
        public void CmdStrike() {
            if (isServer) {
                mainCoaster.GetComponent<CoasterScript>().SetStriked(true);
                gM.Strike();
            }
        }

        [Command]
        public void CmdFake() {
        }

        [Command]
        public void CmdJump() {
            if (isServer)
                bottle.GetComponent<BottleScript>().jumping = true;
        }

        [Command]
        public void CmdFall() {
        }
    }
}
