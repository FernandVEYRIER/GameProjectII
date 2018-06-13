using Assets.Scripts.UI;
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

        /// <summary>
        /// Keeps track of the last game that has been played.
        /// </summary>
        public string LastGamePlayed { get; private set; }

        /// <summary>
        /// Keeps track of the current game being played.
        /// </summary>
        public string CurrentGamePlayed { get; private set; }

        [Header("UI")]
        public RectTransform mainMenuPanel;
        public RectTransform lobbyPanel;
        public RectTransform lobbyPlayerContainer;
        public RectTransform panelLoading;
        public Text loadingText;
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

        private PlayerInfo _localPlayerInfo;

        [SerializeField] private LobbyMenu _lobbyMenu;

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
        /// Displays the loading screen above every menu.
        /// </summary>
        public void DisplayLoadingScreen()
        {
            Debug.Log("Displaying loading screen");
            panelLoading.gameObject.SetActive(true);
        }

        /// <summary>
        /// Called on the instance which is going to host the game.
        /// </summary>
        public override void OnStartHost()
        {
            base.OnStartHost();
            IsHost = true;
            Debug.Log("Starting host !");
            ChangeTo(lobbyPanel);
            backDelegate = StopHostClbk;
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
            if (success == true)
                Debug.Log("success");
            else
                Debug.Log("Fail " + extendedInfo);
            base.OnMatchCreate(success, extendedInfo, matchInfo);
            _currentMatchID = (System.UInt64)matchInfo.networkId;
            if (!success)
                ShowLoadingScreen(false);
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
            Debug.Log("On match joined ! success ? " + success);
            if (!success)
                ShowLoadingScreen(false);
        }

        /// <summary>
        /// Displays the loading screen above every other menu.
        /// </summary>
        /// <param name="active"></param>
        public void ShowLoadingScreen(bool active, string loadingText = "Good luck", float minimalDuration = 0)
        {
            panelLoading.gameObject.SetActive(active);
            this.loadingText.text = loadingText;
            
        }

        /// <summary>
        /// Called by navigation to go back.
        /// </summary>
        public void GoBack()
        {
            Debug.Log("HERE => " + backDelegate.Method);
            ShowLoadingScreen(true);
            backDelegate();
        }

        /// <summary>
        /// Callback action to go back to main menu.
        /// </summary>
        public void SimpleBackClbk()
        {
            ShowLoadingScreen(true);
            ChangeTo(mainMenuPanel);
        }

        /// <summary>
        /// Callback action to go back to the menu without loading screen.
        /// </summary>
        public void SimpleBackNoLoadingClbk()
        {
            ShowLoadingScreen(false);
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

            ShowLoadingScreen(false);

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
            ShowLoadingScreen(false);
            ChangeTo(mainMenuPanel);
        }

        /// <summary>
        /// Called on any error from the client (timeout...)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="errorCode"></param>
        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            Debug.LogWarning("On client error called ! " + errorCode);
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
                ShowLoadingScreen(false);
            }
        }

        /// <summary>
        /// Destroys the match and stop hosting.
        /// </summary>
        public void StopHostClbk()
        {
            LastGamePlayed = "";
            CurrentGamePlayed = "";
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

            ShowLoadingScreen(false);
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
            foreach (var player in lobbySlots)
            {
                var p = player as LobbyPlayer;
                if (p != null)
                {
                    p.RpcDisplayLoading();
                }
            }
            NetworkServer.SetAllClientsNotReady();
            ChangeScene(playScene);
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);
            Debug.Log("Client scene changed on conn " + conn);
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

        /// <summary>
        /// Called on the server when a client is ready.
        /// </summary>
        /// <param name="conn"></param>
        public override void OnServerReady(NetworkConnection conn)
        {
            base.OnServerReady(conn);
            _clientReadyCount++;
            Debug.Log("<<<<<<<<<<<<<<< One client is ready ! " + _clientReadyCount + " >>>>>>>>>>>>>>>");
        }

        /// <summary>
        /// This is called on the server when it is told that a client has finished switching
        /// from the lobby scene to a game player scene.
        /// </summary>
        /// <param name="lobbyPlayer"></param>
        /// <param name="gamePlayer"></param>
        /// <returns></returns>
        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            Debug.Log("<<<<<<<<<<<<<<< One client finished loading the scene ! >>>>>>>>>>>>>>>");
            return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
        }

        /// <summary>
        /// This is called on the server when a networked scene finishes loading.
        /// </summary>
        /// <param name="sceneName"></param>
        public override void OnLobbyServerSceneChanged(string sceneName)
        {
            //_clientReadyCount = 0;
            Debug.Log("on lobby server scene changed " + sceneName);
            base.OnLobbyServerSceneChanged(sceneName);
        }

        /// <summary>
        /// Get the current local player name.
        /// </summary>
        /// <returns></returns>
        public string GetLocalPlayerName()
        {
            return _lobbyMenu.TextPlayerName.text;
        }

        /// <summary>
        /// Perform a scene changing on the server.
        /// </summary>
        /// <param name="scene"></param>
        public void ChangeScene(string scene)
        {
            if (!string.IsNullOrEmpty(CurrentGamePlayed))
                LastGamePlayed = CurrentGamePlayed;
            CurrentGamePlayed = scene;
            Debug.Log("|||||||||||||||||||||||||||||||||||||||||||||||||Last game played = " + LastGamePlayed + " current game played " + CurrentGamePlayed);
            _clientReadyCount = 0;
            ServerChangeScene(scene);
        }

        /// <summary>
        /// Saves player information data.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="id"></param>
        public void SetLocalPlayerInfo(PlayerInfo info)
        {
            _localPlayerInfo = info;
            Debug.Log("Setting player info => " + info.Name + " color = " + info.Color);
        }

        /// <summary>
        /// Retrieves the player info from the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PlayerInfo GetLocalPlayerInfo()
        {
            return _localPlayerInfo;
        }
    }
}