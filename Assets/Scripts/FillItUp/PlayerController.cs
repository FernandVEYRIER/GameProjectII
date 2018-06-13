using System;
using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.FillItUp
{
    public class PlayerController : APlayerControllerEnzo
    {
        [SyncVar(hook = "OnLiquideChange")] public float height;

        [SerializeField] private float speed = 1;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] Text playerText;
        [SerializeField] FillGlass fillGlass;

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
            playerText.enabled = true;

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

        private void OnLiquideChange(float state)
        {
            height = state;
            fillGlass.liquideHeight(state);
        }

        [Command]
        override protected void CmdSetPlayerInfo(PlayerInfo info)
        {
            base.CmdSetPlayerInfo(info);
            print("color of player " + _playerColor);
            _renderer.material.SetColor("_Color", new Color(_playerColor.r, _playerColor.g, _playerColor.b, 0.2f));
        }

        private void Update()
        {
            _renderer.material.SetColor("_Color", new Color(_playerColor.r, _playerColor.g, _playerColor.b, 0.2f));
            if (isLocalPlayer)
            {
                if (ui.GameStart)
                {
                    transform.Translate(Input.acceleration.x * speed * Time.deltaTime, 0, 0);
                    if (transform.position.x < _leftLimit.position.x)
                        transform.position = _leftLimit.position;
                    else if (transform.position.x > _rightLimit.position.x)
                        transform.position = _rightLimit.position;
                }
            }
        }
    }
}
