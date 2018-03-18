using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    /// <summary>
    /// Manages the lobby connections and states.
    /// </summary>
    public class LobbyManager : NetworkLobbyManager
    {
        public static LobbyManager Instance { get; private set; }

        public bool IsHost { get; private set; }

        [Header("UI")]
        public RectTransform mainMenuPanel;

        public RectTransform lobbyPanel;
        public RectTransform lobbyPlayerContainer;
        public Button backButton;

        public delegate void BackButtonDelegate();

        public BackButtonDelegate backDelegate;

        //used to disconnect a client properly when exiting the matchmaker
        [HideInInspector]
        public bool _isMatchmaking;

        protected RectTransform currentPanel;
        protected bool _disconnectServer;
        protected ulong _currentMatchID;

        private const short MsgKicked = MsgType.Highest + 1;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            backButton.gameObject.SetActive(false);
            ChangeTo(mainMenuPanel);
        }

        public override void OnStartHost()
        {
            base.OnStartHost();
            Debug.Log("Starting host !");
            ChangeTo(lobbyPanel);
            backDelegate = StopHostClbk;
            IsHost = true;
        }

        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            Debug.Log("Lobby server create lobby player");
            var p = Instantiate(lobbyPlayerPrefab.gameObject);
            return p;
        }

        public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchCreate(success, extendedInfo, matchInfo);
            _currentMatchID = (System.UInt64)matchInfo.networkId;
        }

        public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchJoined(success, extendedInfo, matchInfo);
            Debug.Log("On match joined !");
        }

        /// <summary>
        /// Called by navigation to go back.
        /// </summary>
        public void GoBack()
        {
            backDelegate();
        }

        public void SimpleBackClbk()
        {
            ChangeTo(mainMenuPanel);
        }

        public void ChangeTo(RectTransform newPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.gameObject.SetActive(false);
            }

            if (newPanel != null)
            {
                newPanel.gameObject.SetActive(true);
            }

            currentPanel = newPanel;

            if (currentPanel != mainMenuPanel)
            {
                backButton.gameObject.SetActive(true);
            }
            else
            {
                backButton.gameObject.SetActive(false);
                _isMatchmaking = false;
            }
        }

        public void KickedMessageHandler(NetworkMessage netMsg)
        {
            netMsg.conn.Disconnect();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            conn.RegisterHandler(MsgKicked, KickedMessageHandler);

            if (!NetworkServer.active)
            {//only to do on pure client (not self hosting client)
                ChangeTo(lobbyPanel);
                backDelegate = StopClientClbk;
            }
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ChangeTo(mainMenuPanel);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            ChangeTo(mainMenuPanel);
        }

        public void StopHostClbk()
        {
            if (_isMatchmaking)
            {
                matchMaker.DestroyMatch((NetworkID)_currentMatchID, 0, OnDestroyMatch);
                _disconnectServer = true;
            }
            else
            {
                StopHost();
            }

            IsHost = false;
            ChangeTo(mainMenuPanel);
        }

        public void StopClientClbk()
        {
            StopClient();

            if (_isMatchmaking)
            {
                StopMatchMaker();
            }

            ChangeTo(mainMenuPanel);
        }

        public string GetPlayerName()
        {
            return mainMenuPanel.GetComponent<LobbyMenu>().TextPlayerName.text;
        }

        public void StartGame()
        {
            ChangeTo(null);
            ServerChangeScene(playScene);
        }

        public override void OnLobbyServerPlayersReady()
        {
        }
    }
}