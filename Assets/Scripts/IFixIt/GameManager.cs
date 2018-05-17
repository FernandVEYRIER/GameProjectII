using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.IFixIt
{
    public class GameManager : AGameManager
    {
        private void Start()
        {
            if (isServer)
                StartCoroutine(WaitForPlayers());
        }

        private IEnumerator WaitForPlayers()
        {
            if (!isServer)
                yield break;

            Debug.Log("Server ready, waiting for players... " + NetworkServer.connections.Count);

            while (!LobbyManager.Instance.AreAllClientsReady)
                yield return null;

            SetGameState(GAME_STATE.Play);
        }
    }
}