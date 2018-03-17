﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    /// <summary>
    /// Manages the lobby player list.
    /// </summary>
    public class LobbyPlayerList : MonoBehaviour
    {
        public static LobbyPlayerList Instance { get; private set; }

        public RectTransform playerListContentTransform;

        protected VerticalLayoutGroup _layout;
        protected List<LobbyPlayer> _players = new List<LobbyPlayer>();


        private void Awake()
        {
            Instance = this;
            _layout = playerListContentTransform.GetComponentInChildren<VerticalLayoutGroup>();
        }

        public void AddPlayer(LobbyPlayer player)
        {
            if (_players.Contains(player))
                return;

            _players.Add(player);

            player.transform.SetParent(playerListContentTransform, false);
            player.transform.localScale = Vector3.one;

            PlayerListModified();
        }

        public void RemovePlayer(LobbyPlayer player)
        {
            _players.Remove(player);
            PlayerListModified();
        }

        public void PlayerListModified()
        {
            int i = 0;
            foreach (LobbyPlayer p in _players)
            {
                //p.OnPlayerListChanged(i);
                ++i;
            }
        }
    }
}