using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Darts
{
    public class CanvasManager : MonoBehaviour
    {
        private RectTransform panelLoadingScreen;
        [SerializeField] private GameManager _manager;
        [SerializeField] private GameObject panelGameOver;
        [SerializeField] private Text _textWinner;

        private void Awake()
        {
            panelLoadingScreen = LobbyManager.Instance.panelLoading;
        }

        // Use this for initialization
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
                    break;

                case Game.GAME_STATE.GameOver:
                    panelGameOver.SetActive(true);
                    _textWinner.text = "";
                    foreach (var player in (AGameManager.Instance as GameManager).Results)
                    {
                        _textWinner.text = player.PlayerName + " " + player.Score.ToString() + "\n";
                    }
                    break;
            }
        }
    }
}