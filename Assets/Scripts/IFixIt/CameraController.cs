using UnityEngine;
using UnityEngine.PostProcessing;

namespace Assets.Scripts.IFixIt
{
    public class CameraController : MonoBehaviour
    {
        public float FocusDistance;
        [SerializeField] private PostProcessingProfile Profile;
        private GameManager _manager;

        private void Update()
        {
            var settings = Profile.depthOfField.settings;
            settings.focusDistance = FocusDistance;
            Profile.depthOfField.settings = settings;
        }

        public void ChangeGameNail()
        {
            if (_manager == null)
                _manager = Game.AGameManager.Instance as GameManager;
            _manager.ChangeMiniGame(0);
        }

        public void ChangeGameSwipe()
        {
            if (_manager == null)
                _manager = Game.AGameManager.Instance as GameManager;
            _manager.ChangeMiniGame(1);
        }

        public void ChangeGameScrew()
        {
            if (_manager == null)
                _manager = Game.AGameManager.Instance as GameManager;
            _manager.ChangeMiniGame(2);
        }
    }
}