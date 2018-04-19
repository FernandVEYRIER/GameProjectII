using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.CantRoachThis
{
    /// <summary>
    /// Player controller for the Cockroach.
    /// </summary>
    public class PlayerController : NetworkBehaviour
    {
        [SyncVar (hook = "OnPlayerNameChange")] public string _playerName;
        [SyncVar(hook = "OnPlayerColorChange")] public Color _playerColor;

        [SerializeField] private float speed = 1;

        private NetworkInstanceId _networkIdentity;
        private CharacterController _controller;

        /// <summary>
        /// Called when the local player is ready.
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            Debug.Log("Starting local player");
            base.OnStartLocalPlayer();
            SetupPlayer();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            OnPlayerColorChange(_playerColor);
            OnPlayerNameChange(_playerName);
        }

        /// <summary>
        /// Sets up the player values.
        /// </summary>
        [Client]
        private void SetupPlayer()
        {
            _networkIdentity = GetComponent<NetworkIdentity>().netId;
            _controller = GetComponent<CharacterController>();
            CmdSetPlayerInfo(LobbyManager.Instance.GetLocalPlayerInfo());
        }

        public override void OnStartServer()
        {
            _networkIdentity = GetComponent<NetworkIdentity>().netId;
            Debug.Log("Requesting id for value => " + _networkIdentity.Value);
            _controller = GetComponent<CharacterController>();
            base.OnStartServer();
        }

        /// <summary>
        /// Called on the server to set the player info.
        /// </summary>
        /// <param name="info"></param>
        [Command]
        private void CmdSetPlayerInfo(PlayerInfo info)
        {
            _playerName = info.Name;
            _playerColor = info.Color;
        }

        /// <summary>
        /// Callback when the playere name changed on the server.
        /// </summary>
        /// <param name="name"></param>
        private void OnPlayerNameChange(string name)
        {
            Debug.Log("Player name changed to == " + name);
            _playerName = name;
        }

        private void OnPlayerColorChange(Color color)
        {
            Debug.Log("Player color changed to == " + color);
            _playerColor = color;
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                var moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;
                transform.Translate(moveDirection /** Time.deltaTime*/);
                //_controller.Move(moveDirection * Time.deltaTime);
            }
        }
    }
}