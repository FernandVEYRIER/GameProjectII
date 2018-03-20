using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class WheelGenerator : MonoBehaviour
    {
        [SerializeField] private RectTransform _wheelContainer;
        [SerializeField] private Image _wheelSlicePrefab;
        [SerializeField] private List<GameEntry> _gameEntries = new List<GameEntry>();

        private void Start()
        {
            if (_gameEntries.Count > 0)
                GenerateWheel();
        }

        private void GenerateWheel()
        {
            float sliceSize = 1f / _gameEntries.Count;
            for (int i = 0; i < _gameEntries.Count; i++)
            {
                var go = Instantiate(_wheelSlicePrefab.gameObject);
                go.transform.SetParent(_wheelContainer.transform);
                go.transform.localScale = Vector3.one;
                go.GetComponent<RectTransform>().localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.Euler(0, 0, 360f * (i / (float)_gameEntries.Count));
                var img = go.GetComponent<Image>();
                img.fillAmount = sliceSize;
                img.color = _gameEntries[i].color;
                go.name = _gameEntries[i].gameScene.name;
            }
        }
    }
}