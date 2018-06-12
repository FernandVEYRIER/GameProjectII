using System;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class EndAnimHandler : MonoBehaviour
    {
        public event EventHandler OnAnimEnd;

        public void LoadLevel()
        {
            if (OnAnimEnd != null)
                OnAnimEnd.Invoke(this, EventArgs.Empty);
        }
    }
}