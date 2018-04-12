using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.CantRoachThis
{
    public class PlayerController : NetworkBehaviour
    {
        [SyncVar (hook = "OnPlayerNameChange")] public string _playerName;

        [SerializeField] private float speed = 1;

        private NetworkInstanceId _networkIdentity;
        private CharacterController _controller;

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
            CmdSetPlayerName(LobbyManager.Instance.GetPlayerName());
            _controller = GetComponent<CharacterController>();
        }

        [Command]
        private void CmdSetPlayerName(string name)
        {
            _playerName = name;
        }

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
                _controller.Move(moveDirection * Time.deltaTime);
            }
        }
    }
}