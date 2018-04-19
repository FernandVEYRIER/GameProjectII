using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    public class LobbyPlayer : NetworkLobbyPlayer
    {
        private static readonly Color[] Colors = new Color[] { Color.magenta, Color.red, Color.cyan, Color.blue, Color.green, Color.yellow, Color.grey, Color.white };
        //used on server to avoid assigning the same color to two player
        private static readonly List<int> _colorInUse = new List<int>();

        [Header("UI")]
        [SerializeField] private Text _textName;
        [SerializeField] private Button _buttonQuit;
        [SerializeField] private Button _colorButton;

        [SyncVar(hook = "HookName")]
        private string _name = "";

        [SyncVar(hook = "HookColor")]
        private Color playerColor = Color.clear;

        public override void OnClientEnterLobby()
        {
            base.OnClientEnterLobby();
            LobbyPlayerList.Instance.AddPlayer(this);
            SetupPlayer(isLocalPlayer);
            HookName(_name);
            HookColor(playerColor);
        }

        private void SetupPlayer(bool isLocal)
        {
            //_buttonQuit.gameObject.SetActive(isLocal);
            _colorButton.interactable = isLocal;
            Debug.Log("IS LOCAL PLAYER ? " + isLocal + " sever ? " + isServer + " is local player var ? " + isLocalPlayer);
            _textName.text = _name;
            //CmdSetName(LobbyManager.Instance.GetPlayerName());
            if (playerColor == Color.clear)
                CmdColorChange();
            _colorButton.onClick.RemoveAllListeners();
            _colorButton.onClick.AddListener(OnColorButtonClick);
        }

        private void OnColorButtonClick()
        {
            CmdColorChange();
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

        public void HookColor(Color newColor)
        {
            playerColor = newColor;
            _colorButton.GetComponent<Image>().color = newColor;
        }

        [Command]
        private void CmdSetName(string name)
        {
            _name = name;
            Debug.Log("Setting CMD name to " + _name);
        }

        [Command]
        public void CmdColorChange()
        {
            int idx = System.Array.IndexOf(Colors, playerColor);

            int inUseIdx = _colorInUse.IndexOf(idx);

            if (idx < 0) idx = 0;

            idx = (idx + 1) % Colors.Length;

            bool alreadyInUse = false;

            do
            {
                alreadyInUse = false;
                for (int i = 0; i < _colorInUse.Count; ++i)
                {
                    if (_colorInUse[i] == idx)
                    {//that color is already in use
                        alreadyInUse = true;
                        idx = (idx + 1) % Colors.Length;
                    }
                }
            }
            while (alreadyInUse);

            if (inUseIdx >= 0)
            {//if we already add an entry in the colorTabs, we change it
                _colorInUse[inUseIdx] = idx;
            }
            else
            {//else we add it
                _colorInUse.Add(idx);
            }

            playerColor = Colors[idx];
        }

        private void OnDestroy()
        {
            LobbyPlayerList.Instance.RemovePlayer(this);

            int idx = System.Array.IndexOf(Colors, playerColor);

            if (idx < 0)
                return;

            for (int i = 0; i < _colorInUse.Count; ++i)
            {
                if (_colorInUse[i] == idx)
                {//that color is already in use
                    _colorInUse.RemoveAt(i);
                    break;
                }
            }
        }
    }
}