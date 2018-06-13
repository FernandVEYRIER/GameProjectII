using Assets.Scripts.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Wheel of fortune procedural generator.
    /// </summary>
    public class WheelGenerator : NetworkBehaviour
    {
        public GameObject TextTitle;
        public EndAnimHandler EndAnimHandler;
        [SerializeField] private Text _textGameChosen;
        [SerializeField] private Text _textShotCount;

        [SerializeField] private GameObject _wheelContainerPrefab;
        [SerializeField] private RectTransform _wheelContainer;
        [SerializeField] private RectTransform _wheelShotContainer;
        [SerializeField] private Image _wheelSlicePrefab;
        [SerializeField] private List<GameEntry> _gameEntries = new List<GameEntry>();
        [SerializeField] private List<int> _shotEntries = new List<int>();

        [SyncVar] private string _lastGamePlayed;

        private void Start()
        {
            Debug.Log("START wheel generator >>>>>>>>>>>> " + isServer);

            if (isServer)
            {
                _lastGamePlayed = LobbyManager.Instance.LastGamePlayed;
                Debug.Log("last game played ====================> " + _lastGamePlayed);
                StartCoroutine(WaitForSpawnWheel());
            }
        }

        /// <summary>
        /// Ensures that every client is connected before sending network messages.
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForSpawnWheel()
        {
            while (!LobbyManager.Instance.AreAllClientsReady)
                yield return null;
            var go = Instantiate(_wheelContainerPrefab);
            //go.transform.SetParent(transform);
            //go.transform.localScale = Vector3.one;
            //go.transform.localPosition = Vector3.zero;
            //go.transform.SetSiblingIndex(0);
            //Debug.Log("Spawning => " + go);
            if (NetworkServer.active)
                NetworkServer.Spawn(go);
            RpcSpawnWheel(go);
        }

        /// <summary>
        /// Retrieves a scene from a given angle.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public string GetCorrespondingScene(Quaternion angle)
        {
            var gameEntriesFiltered = _gameEntries.Where(x => x.gameScene != _lastGamePlayed).ToList();

            // This allows any angle higher than 360 degrees to be computed.
            // The 90 offset is because the arrow is not on top of the wheel.
            var clampedAngle = ((angle.eulerAngles.z + 90f) % 360f);

            var idx = 0;
            var item = 0;
            while (clampedAngle > (idx * (360f / gameEntriesFiltered.Count)) + (360f / _gameEntries.Count))
            {
                ++idx;
                // Item counts backwards because we spin the wheel clockwise
                item = (int)Mathf.Repeat(item - 1, gameEntriesFiltered.Count);
            }
            return gameEntriesFiltered[item].gameScene.SceneName; //should be item don't fuck
        }

        public string GetCorrespondingShots(Quaternion angle)
        {
            // This allows any angle higher than 360 degrees to be computed.
            // The 90 offset is because the arrow is not on top of the wheel.
            var clampedAngle = ((angle.eulerAngles.z + 90f) % 360f);

            var idx = 0;
            var item = 0;
            while (clampedAngle > (idx * (360f / 4f)) + (360f / 4f))
            {
                ++idx;
                item = (int)Mathf.Repeat(item - 1, 4);
            }
            return (item + 1).ToString(); //should be item don't fuck        
        }

        /// <summary>
        /// Makes the client spawn the wheel by replication.
        /// </summary>
        /// <param name="go"></param>
        [ClientRpc]
        private void RpcSpawnWheel(GameObject go)
        {
            Debug.Log("RPC SPAWN WHEEL");
            go.transform.SetParent(transform.GetChild(0).transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            go.transform.SetSiblingIndex(0);
            _wheelContainer = go.GetComponent<RectTransform>();
            go.GetComponent<WheelSpinner>().WheelGenerator = this;
            go.GetComponent<WheelSpinner>().SubWheel = _wheelShotContainer;

            if (_gameEntries.Count > 0)
            {
                GenerateWheel();
                GenerateSubWheel();
            }

            LobbyManager.Instance.ShowLoadingScreen(false);
        }

        /// <summary>
        /// Generate the wheel shot slices.
        /// </summary>
        private void GenerateSubWheel()
        {
            float sliceSize = 1f / _shotEntries.Count;

            for (int i = 0; i < _shotEntries.Count; i++)
            {
                var go = Instantiate(_wheelSlicePrefab.gameObject);
                go.transform.SetParent(_wheelShotContainer.transform);
                go.transform.localScale = Vector3.one / 4f;
                go.GetComponent<RectTransform>().localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.Euler(0, 0, 360f * (i / (float)_shotEntries.Count));
                var img = go.GetComponent<Image>();
                img.fillAmount = sliceSize;
                img.color = (i % 2) == 0 ? Color.grey : Color.white;
                var child = go.transform.GetChild(0);
                child.GetComponent<Text>().text = _shotEntries[i].ToString();
                var rt = child.GetComponent<RectTransform>();
                rt.localRotation = Quaternion.Euler(new Vector3(0, 0, 90f - (360f * sliceSize / 2f)));
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y * 3);
            }
        }

        /// <summary>
        /// Generate the wheel slices.
        /// </summary>
        private void GenerateWheel()
        {
            Debug.Log("last game played RPC ====================> " + _lastGamePlayed);

            var gameEntriesFiltered = _gameEntries.Where(x => x.gameScene != _lastGamePlayed).ToList();
            //Debug.Log("+++++++++++++++++++++++++++++++++++++ Game entries filtered => " + gameEntriesFiltered.Count + " original list => "
            //    + _gameEntries.Count + " last game player => " + LobbyManager.Instance.LastGamePlayed);
            float sliceSize = 1f / gameEntriesFiltered.Count;
            for (int i = 0; i < gameEntriesFiltered.Count; i++)
            {
                var go = Instantiate(_wheelSlicePrefab.gameObject);
                go.transform.SetParent(_wheelContainer.transform);
                go.transform.localScale = Vector3.one;
                go.GetComponent<RectTransform>().localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.Euler(0, 0, 360f * (i / (float)gameEntriesFiltered.Count));
                var img = go.GetComponent<Image>();
                img.fillAmount = sliceSize;
                img.color = gameEntriesFiltered[i].color;
                go.name = gameEntriesFiltered[i].gameScene.SceneName;
                var child = go.transform.GetChild(0);
                child.GetComponent<Text>().text = gameEntriesFiltered[i].gameScene;
                child.GetComponent<RectTransform>().localRotation = Quaternion.Euler(new Vector3(0, 0, 90f - (360f * sliceSize / 2f)));
            }
        }

        public void DisplayChosenGame(string gameName, string shotCount)
        {
            _textGameChosen.transform.parent.gameObject.SetActive(true);
            _textGameChosen.text = gameName;
            _textShotCount.text = shotCount;
        }
    }
}