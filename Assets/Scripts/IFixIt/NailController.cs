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

        private void Awake()
        {
            _initialPos = _nailRect.position;
            _step = _initialPos.y - _target.position.y;
        }

        private void OnEnable()
        {
            _hitRemaining = HitsNeeded;
            _nailRect.position = _initialPos;
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
            }
        }
    }
}