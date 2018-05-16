using System;
using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.Networking;


namespace Assets.Scripts.FillItUp
{
    public class PlayerController : APlayerControllerEnzo
    {
        [SerializeField] private float speed = 1;

        private NetworkInstanceId _networkIdentity;

        private Transform _leftLimit;
        private Transform _rightLimit;
        public bool stopMoving;

        private Assets.Scripts.GoSoju.GameUI ui;

        /// <summary>
        /// Called when the local player is ready.
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            Debug.Log("Starting local player " + _playerColor + "   " + _playerName);
            base.OnStartLocalPlayer();
            SetupPlayer();
            looserDrunk = false;
        }

        /// <summary>
        /// Sets up the player values.
        /// </summary>
        [Client]
        private void SetupPlayer()
        {
            _networkIdentity = GetComponent<NetworkIdentity>().netId;

            var gm = (AGameManager.Instance as GameManager);
            _leftLimit = gm.LeftTerrainLimit;
            _rightLimit = gm.RightTerrainLimit;
            ui = GameObject.Find("PlayerUI").GetComponent<Assets.Scripts.GoSoju.GameUI>();
            ui.SetPlayer(this);
        }

        [Command]
        override protected void CmdSetPlayerInfo(PlayerInfo info)
        {
            base.CmdSetPlayerInfo(info);
            print("color of player " + _playerColor);
            gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", _playerColor);
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                if (ui.GameStart)
                {

                    var dir = (Input.GetMouseButton(0) && Input.mousePosition.x < Screen.width / 2f) ? -1
                    : (Input.GetMouseButton(0) && Input.mousePosition.x >= Screen.width / 2f) ? 1 : 0;
                    var moveDirection = new Vector3(dir, 0, 0);

                    moveDirection = transform.TransformDirection(moveDirection);
                    moveDirection *= speed;
                    transform.Translate(moveDirection /** Time.deltaTime*/);
                    if (transform.position.x < _leftLimit.position.x)
                        transform.position = _leftLimit.position;
                    else if (transform.position.x > _rightLimit.position.x)
                        transform.position = _rightLimit.position;
                }
            }
        }
    }
}
