using UnityEngine;

namespace Assets.Scripts.IFixIt
{
    public class StainManager : MonoBehaviour
    {
        public int StainCount;
        private int _remainingStains;
        [SerializeField] private GameObject _stainPrefab;

        private void OnEnable()
        {
            _remainingStains = StainCount;
            for (int i = 0; i < StainCount; ++i)
            {
                var go = Instantiate(_stainPrefab).GetComponent<RectTransform>();
                go.SetParent(transform);
                go.localScale = Vector3.one;

                var pos = new Vector3(Random.Range(0.1f, 0.9f) * Screen.width, Random.Range(0.1f, 0.9f) * Screen.height, 0);
                go.position = pos;

                go.GetComponent<StainController>().OnPointerExit += StainManager_OnPointerExit;
            }
        }

        private void StainManager_OnPointerExit(object sender, System.EventArgs e)
        {
            var stain = sender as StainController;

            if (stain.SwipesRemaining <= 0)
            {
                stain.OnPointerExit -= StainManager_OnPointerExit;
                Destroy(stain.gameObject);
                --_remainingStains;
                if (_remainingStains <= 0)
                    StainGameOver();
            }
        }

        private void StainGameOver()
        {
            Debug.Log("Stain game over");
        }
    }
}