using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    /// <summary>
    /// Represents one server entry in the Server List.
    /// </summary>
    public class LobbyServerEntry : MonoBehaviour
    {
        [SerializeField] private Text _serverName;
        [SerializeField] private Text _slotInfo;
        [SerializeField] private Button _joinButton;

        /// <summary>
        /// Populates the server entry with the given values.
        /// </summary>
        /// <param name="match"></param>
        /// <param name="lobbyManager"></param>
        /// <param name="c"></param>
        public void Populate(MatchInfoSnapshot match, LobbyManager lobbyManager)
        {
            _serverName.text = match.name;

            _slotInfo.text = match.currentSize.ToString() + "/" + match.maxSize.ToString();

            NetworkID networkID = match.networkId;

            _joinButton.onClick.RemoveAllListeners();
            _joinButton.onClick.AddListener(() => JoinMatch(networkID, lobbyManager));
        }

        /// <summary>
        /// Delegate for the join match button.
        /// </summary>
        /// <param name="networkID"></param>
        /// <param name="lobbyManager"></param>
        private void JoinMatch(NetworkID networkID, LobbyManager lobbyManager)
        {
            lobbyManager.ShowLoadingScreen(true, "Trying to join match " + _serverName.text + " !");
            lobbyManager.matchMaker.JoinMatch(networkID, "", "", "", 0, 0, lobbyManager.OnMatchJoined);
            lobbyManager.backDelegate = lobbyManager.StopClientClbk;
            lobbyManager._isMatchmaking = true;
            //lobbyManager.DisplayIsConnecting();
        }
    }
}