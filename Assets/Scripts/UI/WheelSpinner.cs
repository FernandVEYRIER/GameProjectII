using Assets.Scripts.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Handles the wheel spinning to start the game.
    /// </summary>
    [NetworkSettings(sendInterval = 0.01f)]
    public class WheelSpinner : AGameManager
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
                //LobbyManager.Instance.ChangeScene("Go Soju Go Fast");
                //Networking.LobbyManager.Instance.ChangeScene("Cant Roach This");
#endif

                if (_rb.angularVelocity < 0)
                {
                    //Debug.Log("IS SPINNING VEL => " + _rb.angularVelocity);
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
                    _isSpinning = false;
                    Debug.Log("Stopped spinning ! Chosing game");
                    ChangeScene(WheelGenerator.GetCorrespondingScene(_angle));
                }
                //Debug.Log(_rb.angularVelocity);
                //Debug.Log(WheelGenerator.GetCorrespondingScene(_angle));
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
            Debug.Log("Pointer down");
            _startPos = ((PointerEventData)eventData).position;

            _startDragTime = Time.time;
        }

        [Client]
        public void OnPointerUp(BaseEventData eventData)
        {
            var timeElapsed = Time.time - _startDragTime;
            var posDelta = ((PointerEventData)eventData).position - _startPos;
            Debug.Log("Elapsed time => " + timeElapsed + " Delta pos => " + posDelta);

            if (IsValidSwipe(_startPos, ((PointerEventData)eventData).position, _startDragTime))
            {
                //_isSpinning = true;
                _rb.simulated = true;
                //_rb.angularVelocity = -800;
                CmdSetVelocity(-posDelta.magnitude / timeElapsed / 4f);
            }
        }

        /// <summary>
        /// Check if the processed swipe is considered valid.
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="startDragTime"></param>
        /// <returns></returns>
        private bool IsValidSwipe(Vector2 startPos, Vector2 endPos, float startDragTime)
        {
            return Time.time - startDragTime < 0.3f && Mathf.Abs(endPos.y - startPos.y) > 50f
                && startPos.x > Screen.width / 2 && endPos.x > Screen.width / 2;
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