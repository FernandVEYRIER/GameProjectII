﻿using System;
using System.Linq;
using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.IFixIt
{
    public class CanvasManager : MonoBehaviour
    {
        public static CanvasManager Instance { get; private set; }

        [SerializeField] private GameManager _manager;
        [SerializeField] private GameObject panelGameOver;
        [SerializeField] private GameObject panelWaiting;
        [SerializeField] private Text panelWaitingText;
        [SerializeField] private GameObject panelGame;
        [SerializeField] private GameObject[] panelsMiniGames;
        [SerializeField] private Text _textStart;
        [SerializeField] private Text _textGameOver;

        [SerializeField] private GameObject buttonDrink;

        private RectTransform panelLoadingScreen;

        private void Awake()
        {
            Instance = this;
            panelLoadingScreen = LobbyManager.Instance.panelLoading;
        }

        public void ChangeMiniGame(uint idx)
        {
            for (int i = 0; i < panelsMiniGames.Length; ++i)
            {
                panelsMiniGames[i].SetActive(false);
            }
            panelsMiniGames[idx].SetActive(true);
        }

        public void DisplayWaitingRoom(float totalTime)
        {
            for (int i = 0; i < panelsMiniGames.Length; ++i)
            {
                panelsMiniGames[i].SetActive(false);
            }
            panelGame.SetActive(false);
            panelWaiting.SetActive(true);
            var t = System.TimeSpan.FromSeconds(totalTime);
            panelWaitingText.text = "Your total time is " + string.Format("{0:D1}.{1:D3} s", t.Seconds, t.Milliseconds);
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
                    _textStart.gameObject.SetActive(false);
                    panelLoadingScreen.gameObject.SetActive(false);
                    panelGameOver.SetActive(false);
                    panelGame.SetActive(true);
                    break;

                case Game.GAME_STATE.Pause:
                    break;

                case Game.GAME_STATE.Menu:
                    break;

                case Game.GAME_STATE.GameOver:
                    panelWaiting.SetActive(false);
                    panelGame.SetActive(false);
                    panelGameOver.SetActive(true);
                    buttonDrink.SetActive(Game.AGameManager.Instance.isServer);
                    DisplayPlayerScores();
                    break;

                case Game.GAME_STATE.Loading:
                    break;

                case Game.GAME_STATE.WarmUp:
                    panelLoadingScreen.gameObject.SetActive(false);
                    break;
            }
        }

        private void DisplayPlayerScores()
        {
            var list = _manager.PlayerStatsList.OrderBy(x => x.Time).ToList();
            _textGameOver.text = "";

            for (int i = 0; i < list.Count; i++)
            {
                GameManager.PlayerStats item = list[i];
                var t = TimeSpan.FromSeconds(item.Time);
                //_textGameOver.text += $"{i + 1}. {item.Name}: {string.Format("{0:D1}:{1:D2}.{2:D3} s", t.Minutes, t.Seconds, t.Milliseconds)}\n";
                _textGameOver.text += (i + 1) + ". " + item.Name + ": " + string.Format("{0:D1}:{1:D2}.{2:D3} s", t.Minutes, t.Seconds, t.Milliseconds) + "\n";
            }

            //_textGameOver.text += $"\n{list[list.Count - 1].Name}, you drink !";
            _textGameOver.text += "\n" + list[list.Count - 1].Name + ", you drink !";
        }
    }
}