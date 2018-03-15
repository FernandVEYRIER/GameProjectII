using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Networking
{
    public class LobbyManager : NetworkLobbyManager
    {
        private void OnPlayerConnected(NetworkPlayer player)
        {
            Debug.Log("Player connected ! " + player.ipAddress);
        }

        public override void OnLobbyClientConnect(NetworkConnection conn)
        {
            base.OnLobbyClientConnect(conn);
            Debug.Log("Player CONNECTED " + conn.address);
        }
    }
}