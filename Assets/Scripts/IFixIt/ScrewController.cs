using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.IFixIt
{
    /// <summary>
    /// Screw controller for I Fix It game.
    /// </summary>
    /// https://answers.unity.com/questions/162177/vector2angles-direction.html
    public class ScrewController : MonoBehaviour
    {
        [SerializeField] private RectTransform _screwImage;

        private bool _isInside;
        private Vector2 _startDragPos;
        private Vector2 _center = new Vector2(Screen.width / 2, Screen.height / 2);
        private float _totalRotation;
        public float TargetRotation;
        private float _time;
        private GameManager _gm;
        private Vector3 _screwInitialScale;

        private void Awake()
        {
            _gm = AGameManager.Instance as GameManager;
            _screwInitialScale = _screwImage.localScale;
        }

        private void OnEnable()
        {
            _time = 0;
            _totalRotation = 0;
            _screwImage.localScale = _screwInitialScale;
        }

        private void Update()
        {
            _time += Time.deltaTime;
        }

        public void OnPointerEnter(BaseEventData data)
        {
            //Debug.Log("pointer enter");
            _isInside = true;
        }

        public void OnPointerExit(BaseEventData data)
        {
            //Debug.Log("pointer exit");
            _isInside = false;
        }

        public void OnStartDrag(BaseEventData d)
        {
            var data = d as PointerEventData;
            _startDragPos = data.position;
            //Debug.Log("Start drag pos => " + _startDragPos);
        }

        public void OnPointerDrag(BaseEventData d)
        {
            //Debug.Log("pointer drag");
            if (!_isInside)
                return;
            var data = d as PointerEventData;
            var v1 = _startDragPos - _center;
            var v2 = data.position - _center;
            var angle = Vector2.SignedAngle(v1, v2);
            _screwImage.Rotate(Vector3.forward, angle);
            _startDragPos = data.position;

            _totalRotation += -angle;
            //Debug.Log("angle ====================== > " + angle + " total => " + _totalRotation);

            _screwImage.localScale += Vector3.one * angle * 0.0001f;

            if (_totalRotation >= TargetRotation)
            {
                Debug.Log("Rotation over !!");
                _gm.SetChronoForPlayer(LobbyManager.Instance.GetLocalPlayerInfo().Name, _time);
                gameObject.transform.parent.gameObject.SetActive(false);
                _gm.GoToNextGame();
            }
        }
    }
}