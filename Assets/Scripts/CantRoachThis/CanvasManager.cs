using UnityEngine;

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

        [SerializeField] private GameManager _manager;

        private void Start()
        {
            _manager.OnGameStateChanged += Manager_OnGameStateChanged;
        }

        private void Manager_OnGameStateChanged(object sender, Game.EventGameStatus e)
        {
            switch (e.CurrentState)
            {
                case Game.GAME_STATE.Play:
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

        private void OnDestroy()
        {
            _manager.OnGameStateChanged -= Manager_OnGameStateChanged;
        }
    }
}