using UnityEngine;
using UnityEngine.PostProcessing;

namespace Assets.Scripts.IFixIt
{
    public class CameraController : MonoBehaviour
    {
        public float FocusDistance;
        [SerializeField] private PostProcessingProfile Profile;
        private GameManager _manager;

        private void Start()
        {
            _manager = GameManager.Instance as GameManager;
        }

        private void Update()
        {
            var settings = Profile.depthOfField.settings;
            settings.focusDistance = FocusDistance;
            Profile.depthOfField.settings = settings;
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