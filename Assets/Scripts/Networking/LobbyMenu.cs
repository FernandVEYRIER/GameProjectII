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
        [SerializeField] private Text _textPlayerName;
        [SerializeField] private GameObject _panelLobbyPlayers;
        [SerializeField] private GameObject _panelMenu;
        [SerializeField] private GameObject _lobbyPlayerContainer;
        [SerializeField] private GameObject _panelLobbyFind;
        [SerializeField] private Button[] _buttons;

        private void Start()
        {
            _manager.OnCreatePlayer += LobbyClientConnected;
        }

        /// <summary>
        /// Called when a player joins the lobby.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LobbyClientConnected(object sender, System.EventArgs e)
        {
            var p = Instantiate(_manager.lobbyPlayerPrefab.gameObject);
            p.transform.localScale = Vector3.one;
            p.transform.SetParent(_lobbyPlayerContainer.transform);
        }

        /// <summary>
        /// Called by the UI when pressing the create match button.
        /// </summary>
        public void OnCreateMatchmaking()
        {
            _manager.StartMatchMaker();
            _manager.matchMaker.CreateMatch(_textPlayerName.text, (uint)_manager.maxPlayers, true, "", "", "", 0, 0, _manager.OnMatchCreate);
            DisplayLobbyPanel();
        }

        /// <summary>
        /// Called by the UI when pressing the list server button.
        /// </summary>
        public void OnClickDisplayServerList()
        {
            _manager.StartMatchMaker();
            _panelMenu.SetActive(false);
            _panelLobbyFind.SetActive(true);
        }

        private void DisplayLobbyPanel()
        {
            _panelMenu.SetActive(false);
            _panelLobbyPlayers.SetActive(true);
        }

        private void OnDestroy()
        {
            _manager.OnCreatePlayer -= LobbyClientConnected;
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