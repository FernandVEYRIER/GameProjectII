using UnityEngine;

namespace Assets.Scripts.IFixIt
{
    // Todo : this is called when anim is played in reverse, triggering the game again. Need fix !
    public class CameraController : MonoBehaviour
    {
        private GameManager _manager;

        private void Start()
        {
            _manager = GameManager.Instance as GameManager;
        }

        public void ChangeGameNail()
        {
            _manager.ChangeMiniGame(0);
        }

        public void ChangeGameSwipe()
        {
            _manager.ChangeMiniGame(1);
        }
        public void ChangeGameScrew()
        {
            _manager.ChangeMiniGame(2);
        }
    }
}