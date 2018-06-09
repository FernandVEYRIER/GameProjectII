using System;
using Assets.Scripts.Game;
using UnityEngine;
using UnityEngine.Networking;

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

        public void SetChronoForPlayer(float time)
        {
            CmdSetChronoForPlayer(time);
        }

        [Command]
        private void CmdSetChronoForPlayer(float time)
        {
            if (_manager == null)
                _manager = AGameManager.Instance as GameManager;
            _manager.CmdSetChronoForPlayer(_playerName, time);
        }

        public void NotifyPlayerFinish()
        {
            CmdNotifyPlayerFinish();
        }

        [Command]
        private void CmdNotifyPlayerFinish()
        {
            _manager.CmdNotifyPlayerFinish();
        }
    }
}