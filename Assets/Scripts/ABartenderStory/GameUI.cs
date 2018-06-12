using Assets.Scripts.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Test
{
    public class GameUI : NetworkBehaviour
    {
        public bool GameStart;
        [SerializeField] Text counter;
        [SerializeField] Text winnerUI;
        [SerializeField] Text looserUI;
        [SerializeField] GameObject buttonUI;
        private APlayerControllerEnzo player;

        private void Start()
        {
            Debug.Log("START >>>>>>>>>>>> " + isServer);
            GameStart = false;
            StartCoroutine(Counter());
        }

        public void SetPlayer(APlayerControllerEnzo _player)
        {
            player = _player;
        }

        public void launchCmd()
        {
            player.CmdLooserDrunk();
        }

        private IEnumerator Counter()
        {
            while (!LobbyManager.Instance.AreAllClientsReady)
                yield return null;

            counter.enabled = true;
            counter.text = "3";
            yield return new WaitForSeconds(1);
            counter.text = "2";
            yield return new WaitForSeconds(1);
            counter.text = "1";
            yield return new WaitForSeconds(1);
            if (isServer)
                RpcCounter();
            counter.text = "GO SOJU !";
            yield return new WaitForSeconds(1);
            counter.enabled = false;
            yield return 0;
        }

        [ClientRpc]
        private void RpcCounter()
        {
            GameStart = true;
        }

        [ClientRpc]
        public void RpcSetWinner(string winner)
        {
            GameStart = false;
            winnerUI.enabled = true;
            winnerUI.text = winner;
            buttonUI.SetActive(true);
        }

        [ClientRpc]
        public void RpcSetLooser(string looser)
        {
            looserUI.enabled = true;
            looserUI.text = looser;
        }
    }
}
