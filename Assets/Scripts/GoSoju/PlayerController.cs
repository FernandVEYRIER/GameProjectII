using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Assets.Scripts.Game;
using Assets.Scripts.Networking;

namespace Assets.Scripts.GoSoju
{
    public class PlayerController : APlayerController
    { 
        [SyncVar(hook = "OnPositionChange")] public string position;

        [SerializeField] private float speed = 0.1f;

        private bool right;
        private bool stopMoving;
        private Transform _transform;

        private NetworkInstanceId _networkIdentity;
        private Text myPosition;

        public override void OnStartClient()
        {
            base.OnStartClient();
            position = "position Unknow";
        }

        public override void OnStartLocalPlayer()
        {
            Debug.Log("Starting local player");
            base.OnStartLocalPlayer();
            SetupPlayer();
        }

        [Client]
        private void SetupPlayer()
        {
            _networkIdentity = GetComponent<NetworkIdentity>().netId;
            right = false;
            stopMoving = false;
            _transform = GetComponent<Transform>();
            myPosition = GameObject.Find("ClientPosition").GetComponent<Text>();
            myPosition.text += position;
        }

        [Command]
        override protected void CmdSetPlayerInfo(PlayerInfo info)
        {
            base.CmdSetPlayerInfo(info);
            print("color of player " + _playerColor);
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", _playerColor);
        }

        private void OnPositionChange(string newpos)
        {
            position = newpos;
            if (myPosition != null)
            {
                myPosition.text = "player position : ";
                myPosition.text += newpos;
            }
        }

        // Update is called once per frame
        void Update()
        {
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", _playerColor);
            if (isLocalPlayer && !stopMoving)
            {
                if (Input.GetMouseButton(0) && Input.mousePosition.x <= Screen.width / 2 && right)
                {
                    //if you are touching on the left side
                    right = false;
                    gameObject.transform.position += new Vector3(speed, 0, 0);
                }
                else if (Input.GetMouseButton(0) && Input.mousePosition.x > Screen.width / 2 && !right)
                {
                    //if you are touching on the right side
                    right = true;
                    gameObject.transform.position += new Vector3(speed, 0, 0);
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "glass")
            {
                if (isServer)
                {
                    GameObject.Find("GameManager").GetComponent<GameManager>().playerArrived(_playerName, this);
                }
                if (isLocalPlayer)
                {
                    stopMoving = true;
                }
            }
        }
    }
}
