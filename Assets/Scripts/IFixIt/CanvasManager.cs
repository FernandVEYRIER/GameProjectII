using Assets.Scripts.Networking;
using UnityEngine;

namespace Assets.Scripts.IFixIt
{
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField] private GameManager _manager;
        [SerializeField] private GameObject panelGameOver;
        [SerializeField] private GameObject panelGame;

        [SerializeField] private GameObject buttonDrink;

        private RectTransform panelLoadingScreen;

        private void Awake()
        {
            panelLoadingScreen = LobbyManager.Instance.panelLoading;
        }

        private void Start()
        {
            _manager.OnGameStateChanged += Manager_OnGameStateChanged;
        }

        private void OnDestroy()
        {
            _manager.OnGameStateChanged -= Manager_OnGameStateChanged;
        }

        private void Manager_OnGameStateChanged(object sender, Game.EventGameStatus e)
        {
            switch (e.CurrentState)
            {
                case Game.GAME_STATE.Play:
                    panelLoadingScreen.gameObject.SetActive(false);
                    panelGameOver.SetActive(false);
                    panelGame.SetActive(true);
                    break;

                case Game.GAME_STATE.Pause:
                    break;

                case Game.GAME_STATE.Menu:
                    break;

                case Game.GAME_STATE.GameOver:
                    panelGame.SetActive(false);
                    panelGameOver.SetActive(true);
                    buttonDrink.SetActive(Game.AGameManager.Instance.isServer);
                    break;
            }
        }
    }
}