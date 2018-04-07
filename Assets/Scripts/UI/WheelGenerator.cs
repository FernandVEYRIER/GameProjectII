using Assets.Scripts.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class WheelGenerator : NetworkBehaviour
    {
        [SerializeField] private GameObject _wheelContainerPrefab;
        [SerializeField] private RectTransform _wheelContainer;
        [SerializeField] private Image _wheelSlicePrefab;
        [SerializeField] private List<GameEntry> _gameEntries = new List<GameEntry>();

        private int _connectedPlayerCount;

        public override void OnStartLocalPlayer()
        {
            Debug.Log("START >>>>>>>>>>>> " + isServer);
            return;

            base.OnStartLocalPlayer();
            if (isServer)
            {
                var go = Instantiate(_wheelContainerPrefab);
                //go.transform.SetParent(transform);
                //go.transform.localScale = Vector3.one;
                //go.transform.localPosition = Vector3.zero;
                //go.transform.SetSiblingIndex(0);
                Debug.Log("Spawning => " + go);
                if (NetworkServer.active)
                    NetworkServer.Spawn(go);
                RpcSpawnWheel(go);
            }
        }

        private void Start()
        {
            Debug.Log("START >>>>>>>>>>>> " + isServer);

            if (isServer)
            {
                StartCoroutine(WaitForSpawnWheel());
            }
        }

        private IEnumerator WaitForSpawnWheel()
        {
            while (!LobbyManager.Instance.AreAllClientsReady)
                yield return null;
            var go = Instantiate(_wheelContainerPrefab);
            //go.transform.SetParent(transform);
            //go.transform.localScale = Vector3.one;
            //go.transform.localPosition = Vector3.zero;
            //go.transform.SetSiblingIndex(0);
            Debug.Log("Spawning => " + go);
            if (NetworkServer.active)
                NetworkServer.Spawn(go);
            RpcSpawnWheel(go);
        }

        [ClientRpc]
        private void RpcSpawnWheel(GameObject go)
        {
            Debug.Log("RPC SPAWN WHEEL");
            go.transform.SetParent(transform.GetChild(0).transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.SetSiblingIndex(0);
            _wheelContainer = go.GetComponent<RectTransform>();

            if (_gameEntries.Count > 0)
                GenerateWheel();
        }

        private void GenerateWheel()
        {
            float sliceSize = 1f / _gameEntries.Count;
            for (int i = 0; i < _gameEntries.Count; i++)
            {
                var go = Instantiate(_wheelSlicePrefab.gameObject);
                go.transform.SetParent(_wheelContainer.transform);
                go.transform.localScale = Vector3.one;
                go.GetComponent<RectTransform>().localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.Euler(0, 0, 360f * (i / (float)_gameEntries.Count));
                var img = go.GetComponent<Image>();
                img.fillAmount = sliceSize;
                img.color = _gameEntries[i].color;
                go.name = _gameEntries[i].gameScene.name;
            }
        }
    }
}