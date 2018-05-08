using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Game
{
    /// <summary>
    /// Abstract class for player controller.
    /// </summary>
    public abstract class APlayerController : NetworkBehaviour
    {
        [SyncVar(hook = "OnPlayerNameChange")] public string _playerName;
        [SyncVar(hook = "OnPlayerColorChange")] public Color _playerColor;

        /// <summary>
        /// Called when the local player is ready.
        /// </summary>
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            SetupPlayer();
        }

        /// <summary>
        /// Called on every NetworkBehaviour when it is activated on a client.
        /// </summary>
        public override void OnStartClient()
        {
            base.OnStartClient();
            OnPlayerColorChange(_playerColor);
            OnPlayerNameChange(_playerName);
        }

        [Client]
        private void SetupPlayer()
        {
            CmdSetPlayerInfo(LobbyManager.Instance.GetLocalPlayerInfo());
        }

        /// <summary>
        /// Called on the server to set the player info.
        /// </summary>
        /// <param name="info"></param>
        [Command]
        virtual protected void CmdSetPlayerInfo(PlayerInfo info)
        {
            _playerName = info.Name;
            _playerColor = info.Color;

        }

        /// <summary>
        /// Callback when the player name changed on the server.
        /// </summary>
        /// <param name="name"></param>
        protected void OnPlayerNameChange(string name)
        {
            _playerName = name;
        }

        /// <summary>
        /// Callback when the player color changed on the server.
        /// </summary>
        /// <param name="color"></param>
        protected virtual void OnPlayerColorChange(Color color)
        {
            _playerColor = color;
        }
    }
}
