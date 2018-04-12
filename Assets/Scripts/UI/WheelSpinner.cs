using Assets.Scripts.Networking;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace Assets.Scripts.UI
{
    [NetworkSettings(sendInterval = 0.01f)]
    public class WheelSpinner : NetworkBehaviour
    {
        [SyncVar(hook = "HookAngleChanged")] private Quaternion _angle;

        public WheelGenerator WheelGenerator;

        private float _startDragTime;
        private Vector2 _startPos;

        private bool _isSpinning;
        private Rigidbody2D _rb;

        private float _currVel;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.angularVelocity = 0;
            //_rb.simulated = _isSpinning;
        }

        private void Update()
        {
            UpdateAngularVelocity();
        }

        private void UpdateAngularVelocity()
        {
            if (isServer)
            {
#if !RELEASE
                LobbyManager.Instance.ServerChangeScene("Cant Roach This");
#endif

                if (_rb.angularVelocity < 0)
                {
                    Debug.Log("IS SPINNING VEL => " + _rb.angularVelocity);
                    _isSpinning = true;
                    _rb.angularVelocity = Mathf.SmoothDamp(_rb.angularVelocity, 0, ref _currVel, Time.deltaTime * 150);
                    _angle = GetComponent<RectTransform>().localRotation;
                    //Debug.Log(_angle);
                }
                else
                {
                    _rb.angularVelocity = 0;
                }
                if (_rb.angularVelocity == 0 && _isSpinning)
                {
                    Debug.Log("Stopped spinning ! Chosing game");
                    LobbyManager.Instance.ServerChangeScene(WheelGenerator.GetCorrespondingScene(_angle));
                }
                //Debug.Log(_rb.angularVelocity);
                Debug.Log(WheelGenerator.GetCorrespondingScene(_angle));
            }
            if (isClient)
            {
                var t = GetComponent<RectTransform>();
                //Debug.Log(_angle + " Rotating by " + Quaternion.RotateTowards(t.localRotation, _angle, Time.deltaTime * 360f));
                t.localRotation = Quaternion.Lerp(t.localRotation, _angle, 0.1f);
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
            //Debug.Log("angle hook ! " + angle);
            _angle = angle;

            //var t = GetComponent<RectTransform>();
            //t.localRotation = angle;
        }
    }
}