using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        [Header("UI")]
        [SerializeField] private Text _textName;
        [SerializeField] private Button _buttonQuit;

        [SyncVar(hook = "HookName")]
        private string _name = "";

        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();
            LobbyPlayerList.Instance.AddPlayer(this);
            SetupPlayer(isLocalPlayer);
        }

        private void SetupPlayer(bool isLocal)
        {
            _buttonQuit.gameObject.SetActive(isLocal);
            Debug.Log("IS LOCAL PLAYER ? " + isLocal + " sever ? " + isServer + " is local player var ? " + isLocalPlayer);
            _textName.text = _name;
            //CmdSetName(LobbyManager.Instance.GetPlayerName());
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            Debug.Log("On start authority !!!");
            CmdSetName(LobbyManager.Instance.GetPlayerName());
            SetupPlayer(true);
        }

        public override void OnStartLocalPlayer()
        {
            Debug.Log("On start local player !!!");
            //CmdSetName(LobbyManager.Instance.GetPlayerName());
            base.OnStartLocalPlayer();
            SendReadyToBeginMessage();
        }

        public void SetName(string name)
        {
            _name = name;
            _textName.text = _name;
        }

        private void HookName(string name)
        {
            Debug.Log("Name changed to " + name);
            _textName.text = name;
        }

        [Command]
        private void CmdSetName(string name)
        {
            _name = name;
            Debug.Log("Setting CMD name to " + _name);
        }

        private void OnDestroy()
        {
            LobbyPlayerList.Instance.RemovePlayer(this);
        }
    }
}