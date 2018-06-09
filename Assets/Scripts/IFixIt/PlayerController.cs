using System;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.IFixIt
{
    public class PlayerController : APlayerController
    {
        private GameManager _manager;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            _manager = AGameManager.Instance as GameManager;
            _manager.RegisterLocalClient(this);
        }

        internal void SetChronoForPlayer(float time)
        {
            _manager.CmdSetChronoForPlayer(_playerName, time);
        }
    }
}