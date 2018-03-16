using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;

namespace Assets.Scripts.Networking
{
    public class LobbyServerList : MonoBehaviour
    {
        [SerializeField] private RectTransform _serverList;
        [SerializeField] private GameObject _serverInfoPrefab;

        private LobbyManager _manager;

        private void OnEnable()
        {
            _manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LobbyManager>();

            foreach (Transform server in _serverList)
            {
                Destroy(server.gameObject);
            }
            RefreshList();
        }

        private void RefreshList()
        {
            _manager.matchMaker.ListMatches(0, 100, "", false, 0, 0, OnGUIMatchList);
        }

        private void OnGUIMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> responseData)
        {
            foreach (var info in responseData)
            {
                Debug.Log("got info " + info.name);
                var go = Instantiate(_serverInfoPrefab);
                go.transform.SetParent(_serverList);
            }
        }
    }
}