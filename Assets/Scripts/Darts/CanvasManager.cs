using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Networking;

namespace Assets.Scripts.Darts
{
    public class CanvasManager : MonoBehaviour
    {

        private RectTransform panelLoadingScreen;
        [SerializeField] private GameManager _manager;

        private void Awake()
        {
            panelLoadingScreen = LobbyManager.Instance.panelLoading;
        }

        // Use this for initialization
        void Start()
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
            }
        }

                    // Update is called once per frame
                    void Update()
        {

        }
    }
}
