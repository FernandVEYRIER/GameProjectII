using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.CantRoachThis
{
    /// <summary>
    /// Manager for the canvases in the Cockroach game.
    /// </summary>
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField] private GameObject panelGameOver;
        [SerializeField] private GameObject panelGame;
        [SerializeField] private GameObject buttonDrink;
        [SerializeField] private Text textWinner;

        [SerializeField] private GameManager _manager;

        private RectTransform panelLoadingScreen;

        private void Awake()
        {
            panelLoadingScreen = LobbyManager.Instance.panelLoading;
        }

        private void Start()
        {
            _manager.OnGameStateChanged += Manager_OnGameStateChanged;
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
                    GenerateWinnerText();
                    break;
            }
        }

        private void GenerateWinnerText()
        {
            var winnerList = _manager.PlayersDead;

            textWinner.text = "";
            for (int i = 0; i < winnerList.Count; ++i)
            {
                textWinner.text += $"{i + 1}. {winnerList[i]}{(i < winnerList.Count ? "\n" : "")}";
            }
        }

        private void OnDestroy()
        {
            _manager.OnGameStateChanged -= Manager_OnGameStateChanged;
        }
    }
}