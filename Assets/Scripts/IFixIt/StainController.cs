using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.IFixIt
{
    public class StainController : MonoBehaviour
    {
        public int SwipesUntilErased;

        public event EventHandler OnPointerExit;

        [SerializeField] private Image _image;
        public int SwipesRemaining { get; set; }

        private void Start()
        {
            SwipesRemaining = SwipesUntilErased;
        }

        public void PointerExit()
        {
            SwipesRemaining--;
            var color = _image.color;
            color.a = SwipesRemaining / (float)SwipesUntilErased;
            _image.color = color;
            Debug.Log("image color = " + _image.color);
            if (OnPointerExit != null)
                OnPointerExit.Invoke(this, EventArgs.Empty);
        }
    }
}