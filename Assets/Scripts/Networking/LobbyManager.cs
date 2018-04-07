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

        /// <summary>
        /// Is this instance the hosting one ?
        /// </summary>
        public bool IsHost { get; private set; }

        public int ConnectionCount { get { return NetworkServer.connections.Count; } }

        public bool AreAllClientsReady { get { return NetworkServer.connections.Count == _clientReadyCount; } }

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

        private int _clientReadyCount;

        /// <summary>
        /// Initializes the Singleton for this instance.
        /// <para/>
        /// IMPORTANT NOTE : do not use the Awake method inside network derived scripts.
        /// This is going to mess everything up and break scripts.
        /// </summary>
        private void Initialize()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            else
                Instance = this;
        }

        private void Start()
        {
            Initialize();
            backButton.gameObject.SetActive(false);
            ChangeTo(mainMenuPanel);
        }

        /// <summary>
        /// Called on the instance which is going to host the game.
        /// </summary>
        public override void OnStartHost()
        {
            base.OnStartHost();
            Debug.Log("Starting host !");
            ChangeTo(lobbyPanel);
            backDelegate = StopHostClbk;
            IsHost = true;
        }

        /// <summary>
        /// Called everytime a player joins the lobby, server side.
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="playerControllerId"></param>
        /// <returns></returns>
        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            Debug.Log("Lobby server create lobby player");
            var p = Instantiate(lobbyPlayerPrefab.gameObject);
            return p;
        }

        /// <summary>
        /// Called when a match is created, which is when the matchmaking is invoked.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="extendedInfo"></param>
        /// <param name="matchInfo"></param>
        public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchCreate(success, extendedInfo, matchInfo);
            _currentMatchID = (System.UInt64)matchInfo.networkId;
        }

        /// <summary>
        /// Caled when a player joins a match created thanks to matchmaking.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="extendedInfo"></param>
        /// <param name="matchInfo"></param>
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

        /// <summary>
        /// Callback action to go back to main menu.
        /// </summary>
        public void SimpleBackClbk()
        {
            ChangeTo(mainMenuPanel);
        }

        /// <summary>
        /// Allow navigation between different panels, activating
        /// the given one and deactivating the current one.
        /// </summary>
        /// <param name="newPanel"></param>
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

        /// <summary>
        /// Handler when a player gets kicked from the lobby room.
        /// </summary>
        /// <param name="netMsg"></param>
        public void KickedMessageHandler(NetworkMessage netMsg)
        {
            netMsg.conn.Disconnect();
        }

        /// <summary>
        /// Called when a client connects to the server.
        /// </summary>
        /// <param name="conn"></param>
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

        /// <summary>
        /// Called when a client disconnects from the server.
        /// </summary>
        /// <param name="conn"></param>
        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ChangeTo(mainMenuPanel);
        }

        /// <summary>
        /// Called on any error from the client (timeout...)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="errorCode"></param>
        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            ChangeTo(mainMenuPanel);
        }

        /// <summary>
        /// Called when the matchmaking is destroyed.
        /// </summary>
        /// <param name="success"></param>
        /// <param name="extendedInfo"></param>
        public override void OnDestroyMatch(bool success, string extendedInfo)
        {
            base.OnDestroyMatch(success, extendedInfo);
            if (_disconnectServer)
            {
                StopMatchMaker();
                StopHost();
            }
        }

        /// <summary>
        /// Destroys the match and stop hosting.
        /// </summary>
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

        /// <summary>
        /// Stops matchmaker and client.
        /// </summary>
        public void StopClientClbk()
        {
            StopClient();

            if (_isMatchmaking)
            {
                StopMatchMaker();
            }

            ChangeTo(mainMenuPanel);
        }

        /// <summary>
        /// Retrieves the player name from the field input.
        /// </summary>
        /// <returns></returns>
        public string GetPlayerName()
        {
            return mainMenuPanel.GetComponent<LobbyMenu>().TextPlayerName.text;
        }

        /// <summary>
        /// Starts the game by loading the game scene.
        /// </summary>
        public void StartGame()
        {
            NetworkServer.SetAllClientsNotReady();
            ServerChangeScene(playScene);
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0)
                ChangeTo(null);
        }

        /// <summary>
        /// Called when every client notifies the server that they are ready.
        /// Overriden so the match doesnt start automatically.
        /// </summary>
        public override void OnLobbyServerPlayersReady()
        {
            Debug.Log("ALL READY TO GOOOO");
        }

        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            _clientReadyCount++;
            Debug.Log("<<<<<<<<<<<<<<< One client is ready ! >>>>>>>>>>>>>>>");
        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            Debug.Log("<<<<<<<<<<<<<<< One client finished loading the scene ! >>>>>>>>>>>>>>>");
            return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
        }

        public override void OnLobbyServerSceneChanged(string sceneName)
        {
            _clientReadyCount = 0;
            base.OnLobbyServerSceneChanged(sceneName);
        }
    }
}