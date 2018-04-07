using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.UI
{
    public class CanvasSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject _canvas;

        private void Start()
        {
            _canvas = Instantiate(_canvas);
            //NetworkServer.Spawn(_canvas);
        }
    }
}