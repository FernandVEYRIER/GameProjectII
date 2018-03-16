using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
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

        private void Start()
        {
            _manager.OnCreatePlayer += LobbyClientConnected;
        }

        private void LobbyClientConnected(object sender, System.EventArgs e)
        {
            var p = Instantiate(_manager.lobbyPlayerPrefab.gameObject);
            p.transform.localScale = Vector3.one;
            p.transform.SetParent(_lobbyPlayerContainer.transform);
        }

        public void OnCreateMatchmaking()
        {
            _manager.StartMatchMaker();
            _manager.matchMaker.CreateMatch(_textPlayerName.text, (uint)_manager.maxPlayers, true, "", "", "", 0, 0, _manager.OnMatchCreate);
            DisplayLobbyPanel();
        }

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
    }
}