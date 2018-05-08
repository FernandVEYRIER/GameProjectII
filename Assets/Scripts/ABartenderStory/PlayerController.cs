using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.ABartenderStory {
    /// <summary>
    /// Player controller for the Cockroach.
    /// </summary>
    public class PlayerController : APlayerController {
        [SerializeField] private float speed = 1;

        private NetworkInstanceId _networkIdentity;
        private CharacterController _controller;

        /// <summary>
        /// Called when the local player is ready.
        /// </summary>
        public override void OnStartLocalPlayer() {
            Debug.Log("Starting local player");
            base.OnStartLocalPlayer();
            SetupPlayer();
        }

        //public override void OnStartClient()
        //{
        //    base.OnStartClient();
        //    OnPlayerColorChange(_playerColor);
        //    OnPlayerNameChange(_playerName);
        //}

        /// <summary>
        /// Sets up the player values.
        /// </summary>
        [Client]
        private void SetupPlayer() {
            _networkIdentity = GetComponent<NetworkIdentity>().netId;
            _controller = GetComponent<CharacterController>();
            //CmdSetPlayerInfo(LobbyManager.Instance.GetLocalPlayerInfo());
        }

        public override void OnStartServer() {
            _networkIdentity = GetComponent<NetworkIdentity>().netId;
            Debug.Log("Requesting id for value => " + _networkIdentity.Value);
            _controller = GetComponent<CharacterController>();
            base.OnStartServer();
        }

        [Server]
        private void Update() {
            if (isLocalPlayer) {
                var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;
                transform.Translate(moveDirection /** Time.deltaTime*/);
                //_controller.Move(moveDirection * Time.deltaTime);
            }
        }
    }
}
