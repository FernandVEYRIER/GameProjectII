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
            _textName.text = LobbyManager.Instance.GetPlayerName();
            CmdSetName();
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            Debug.Log("On start authority !!!");
            CmdSetName();
            SetupPlayer(true);
        }

        public override void OnStartLocalPlayer()
        {
            Debug.Log("On start local player !!!");
            CmdSetName();
            base.OnStartLocalPlayer();
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
        private void CmdSetName()
        {
            Debug.LogError("toto lol ici");
            if (isLocalPlayer)
            {
                _name = LobbyManager.Instance.GetPlayerName();
                Debug.Log("Setting CMD name to " + _name);
            }
        }
    }
}