using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Networking
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField]
        protected LobbyManager _manager;

        [SerializeField]
        private Text _textPlayerName;

        public void OnCreateMatchmaking()
        {
            _manager.StartMatchMaker();
            _manager.matchMaker.CreateMatch(_textPlayerName.text, 8, true, "", "", "", 0, 0, _manager.OnMatchCreate);
        }

        public void OnClickDisplayServerList()
        {
            _manager.StartMatchMaker();
        }
    }
}