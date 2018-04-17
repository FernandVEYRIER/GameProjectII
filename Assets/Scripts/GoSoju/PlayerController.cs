using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Networking;

namespace Assets.Scripts.GoSoju
{
    public class PlayerController : NetworkBehaviour
    { 
        [SyncVar(hook = "OnPlayerNameChange")] public string _playerName;

        [SerializeField] private float speed = 1;

        private NetworkInstanceId _networkIdentity;

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

        // Update is called once per frame
        void Update()
        {

        }
    }
}
