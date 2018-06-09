using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.IFixIt
{
    public class NailController : MonoBehaviour
    {
        public int HitsNeeded;
        private int _hitRemaining;

        [SerializeField] private RectTransform _nailRect;
        [SerializeField] private RectTransform _target;
        private Vector3 _initialPos;
        private float _step;

        private GameManager _gm;
        private float _time = 0;

        private void Awake()
        {
            _gm = AGameManager.Instance as GameManager;
            _initialPos = _nailRect.position;
            _step = _initialPos.y - _target.position.y;
        }

        private void OnEnable()
        {
            _time = 0;
            _hitRemaining = HitsNeeded;
            _nailRect.position = _initialPos;
        }

        private void Update()
        {
            _time += Time.deltaTime;
        }

        public void OnPointerDown(BaseEventData data)
        {
            _hitRemaining--;
            var y = _target.position.y + _step * (_hitRemaining / (float)HitsNeeded);
            Debug.Log(y);
            _nailRect.position = new Vector3(_nailRect.position.x, y, 1);

            if (_hitRemaining == 0)
            {
                Debug.Log("Nail game ended");
                _gm.CmdSetChronoForPlayer(LobbyManager.Instance.GetLocalPlayerInfo().Name, _time);
                gameObject.transform.parent.gameObject.SetActive(false);
                _gm.GoToNextGame();
            }
        }
    }
}