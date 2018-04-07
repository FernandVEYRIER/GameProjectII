using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace Assets.Scripts.UI
{
    public class WheelSpinner : NetworkBehaviour
    {
        [SyncVar(hook = "HookAngleChanged")] private Quaternion _angle;

        private float _startDragTime;
        private Vector2 _startPos;

        private bool _isSpinning;
        private Rigidbody2D _rb;

        private float _currVel;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            //_rb.simulated = _isSpinning;
        }

        private void Update()
        {
            UpdateAngularVelocity();
            //if (_rb.angularVelocity < 0)
            //{
            //    _rb.angularVelocity = Mathf.SmoothDamp(_rb.angularVelocity, 0, ref _currVel, Time.deltaTime * 150);
            //    _angle = GetComponent<RectTransform>().localRotation;
            //    //Debug.Log(_angle);
            //}
            //else
            //{
            //    _rb.angularVelocity = 0;
            //}
        }

        private void UpdateAngularVelocity()
        {
            if (isServer && _rb.angularVelocity < 0)
            {
                _rb.angularVelocity = Mathf.SmoothDamp(_rb.angularVelocity, 0, ref _currVel, Time.deltaTime * 150);
                _angle = GetComponent<RectTransform>().localRotation;
                //Debug.Log(_angle);
            }
        }

        [Client]
        public void OnPointerDown(BaseEventData eventData)
        {
            _startPos = ((PointerEventData)eventData).position;

            _startDragTime = Time.time;
        }

        [Client]
        public void OnPointerUp(BaseEventData eventData)
        {
            var timeElapsed = Time.time - _startDragTime;
            var posDelta = ((PointerEventData)eventData).position - _startPos;
            Debug.Log("Elapsed time => " + timeElapsed + " Delta pos => " + posDelta);
            if (!_isSpinning && timeElapsed < 0.2f && Mathf.Abs(posDelta.y) > 50)
            {
                //_isSpinning = true;
                _rb.simulated = true;
                //_rb.angularVelocity = -800;
                CmdSetVelocity(-800);
            }
        }

        [Client]
        public void OnDrag(BaseEventData eventData)
        {
        }

        [Command]
        private void CmdSetVelocity(float vel)
        {
            Debug.Log("CMD SET VAL");
            _rb.angularVelocity = vel;
        }

        private void HookAngleChanged(Quaternion angle)
        {
            Debug.Log("angle hook ! " + angle);
            var t = GetComponent<RectTransform>();
            t.localRotation = angle;
        }
    }
}