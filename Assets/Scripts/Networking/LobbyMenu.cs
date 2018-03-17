using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    /// <summary>
    /// Handles the Main Lobby menu UI.
    /// </summary>
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField]
        protected LobbyManager _manager;

        [Header("UI")]
        [SerializeField] public Text TextPlayerName;
        [SerializeField] private GameObject _panelLobbyPlayers;
        [SerializeField] private GameObject _panelMenu;
        [SerializeField] private GameObject _lobbyPlayerContainer;
        [SerializeField] private GameObject _panelLobbyFind;
        [SerializeField] private Button[] _buttons;

        private void MatchJoined(object sender, System.EventArgs e)
        {
            _panelLobbyPlayers.SetActive(true);
            _panelLobbyFind.SetActive(false);
        }

        /// <summary>
        /// Called when a player joins the lobby.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LobbyClientConnected(object sender, System.EventArgs e)
        {
            var p = Instantiate(_manager.lobbyPlayerPrefab.gameObject);
            p.GetComponent<LobbyPlayer>().SetName(TextPlayerName.text);
            p.transform.SetParent(_lobbyPlayerContainer.transform);
            p.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Called by the UI when pressing the create match button.
        /// </summary>
        public void OnCreateMatchmaking()
        {
            _manager.StartMatchMaker();
            _manager.matchMaker.CreateMatch(TextPlayerName.text, (uint)_manager.maxPlayers, true, "", "", "", 0, 0, _manager.OnMatchCreate);
            _manager.backDelegate = _manager.StopHost;
            _manager._isMatchmaking = true;
            //DisplayLobbyPanel();
        }

        /// <summary>
        /// Called by the UI when pressing the list server button.
        /// </summary>
        public void OnClickDisplayServerList()
        {
            _manager.StartMatchMaker();
            _manager.backDelegate = _manager.SimpleBackClbk;
            _manager.ChangeTo(_panelLobbyFind.GetComponent<RectTransform>());
        }

        private void DisplayLobbyPanel()
        {
            _panelMenu.SetActive(false);
            _panelLobbyPlayers.SetActive(true);
        }

        /// <summary>
        /// Called by the input field everytime a character changes.
        /// </summary>
        /// <param name="str"></param>
        public void ValidateForm(string str)
        {
            foreach (var button in _buttons)
                button.interactable = !string.IsNullOrEmpty(str);
        }
    }
}