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
            SetupPlayer(isLocalPlayer);
        }

        private void SetupPlayer(bool isLocalPlayer)
        {
            _buttonQuit.enabled = isLocalPlayer;
        }

        public void SetName(string name)
        {
            _name = name;
        }

        private void HookName(string name)
        {
            Debug.Log("Name changed");
            _textName.text = name;
        }
    }
}