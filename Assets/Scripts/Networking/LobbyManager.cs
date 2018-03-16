using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    /// <summary>
    /// Manages the lobby connections and states.
    /// </summary>
    public class LobbyManager : NetworkLobbyManager
    {
        public event EventHandler OnCreatePlayer;

        public override void OnStartHost()
        {
            base.OnStartHost();
            Debug.Log("Starting host !");
        }

        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            Debug.Log("Lobby server create lobby player");
            if (OnCreatePlayer != null)
                OnCreatePlayer.Invoke(this, EventArgs.Empty);
            return base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
        }

        /// <summary>
        /// Called by navigation to go back.
        /// </summary>
        public void GoBack()
        {

        }
    }
}