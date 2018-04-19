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

        /// <summary>
        /// Sets up the player values.
        /// </summary>
        [Client]
        private void SetupPlayer()
        {
            _networkIdentity = GetComponent<NetworkIdentity>().netId;
            CmdSetPlayerName(LobbyManager.Instance.GetPlayerName());
            _controller = GetComponent<CharacterController>();
        }

        /// <summary>
        /// Called on the server to set the name.
        /// </summary>
        /// <param name="name"></param>
        [Command]
        private void CmdSetPlayerName(string name)
        {
            _playerName = name;
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