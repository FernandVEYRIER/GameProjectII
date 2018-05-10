using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Darts
{
    public class GameManager : AGameManager
    {
        private void Start()
        {
            if (isServer)
                StartCoroutine(SpawnObjects());
        }

        private void Update()
        {
        }

        [Server]
        private IEnumerator SpawnObjects()
        {
            if (!isServer)
                yield break;

            Debug.Log("Server ready, waiting for players... " + NetworkServer.connections.Count);

            while (!LobbyManager.Instance.AreAllClientsReady)
                yield return null;

            Debug.Log("Is server ? " + NetworkServer.active);
            SetGameState(GAME_STATE.Play);
        }
    }
}