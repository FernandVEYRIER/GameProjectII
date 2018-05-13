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
                    Result[] result = new Result[(AGameManager.Instance as GameManager).Results.Count];
                    (AGameManager.Instance as GameManager).Results.CopyTo(result, 0);
                    int i = 1;
                    while(i < result.Length && result.Length >= 2)
                    {
                        if (result[i - 1].Score < result[i].Score)
                        {
                            var tmp = result[i - 1];
                            result[i - 1] = result[i];
                            result[i] = tmp;
                            i = 1;
                        }
                        else
                            ++i;
                    }
                    i = 1;
                    foreach (var player in result)
                    {
                        _textWinner.text += i.ToString() + ". " + player.PlayerName + " " + player.Score.ToString() + "\n";
                        ++i;
                    }
                    break;
            }
        }
    }
}