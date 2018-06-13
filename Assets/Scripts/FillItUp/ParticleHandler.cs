using System;
using UnityEngine;

namespace Assets.Scripts.Liquids
{
    public delegate void EventParticle(object sender, GameObject obj);

    /// <summary>
    /// Handles the particle behaviour to simulate liquids.
    /// </summary>
    public class ParticleHandler : MonoBehaviour
    {
        public event EventParticle OnParticleCollided;

        private void OnParticleCollision(GameObject other)
        {
            if (OnParticleCollided != null)
                OnParticleCollided.Invoke(this, other);
        }
    }
}