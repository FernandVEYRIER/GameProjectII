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
        /*[Tooltip("The transform to watch for rotations")]
        [SerializeField] private Transform relativeTransform;*/

        /*[SerializeField] private ObiSolver _solver;

        //private void Awake()
        //{
        //    _particles = GetComponent<ParticleSystem>();
        //}

        private void OnEnable()
        {
           // Debug.Log("solver = ");
            _solver.OnCollision += Solver_OnCollision;
        }

        private void OnDisable()
        {
            _solver.OnCollision -= Solver_OnCollision;
        }

        private void Solver_OnCollision(object sender, ObiSolver.ObiCollisionEventArgs e)
        {
//            Debug.Log("collision obi");
            if (OnParticleCollided != null)
            {
                foreach (var item in e.contacts)
                {
                    Component collider;
                    if (ObiCollider.idToCollider.TryGetValue(item.other, out collider))
                    {
                        OnParticleCollided.Invoke(this, collider.gameObject);
                    }
                }
            }
        }*/

        /// <summary>
        /// Invoked when particles collide with something.
        /// </summary>
        public event EventParticle OnParticleCollided;

        //private void OnParticleCollision(GameObject other)
        //{
        //    if (OnParticleCollided != null)
        //        OnParticleCollided.Invoke(this, other);
        //}

        //private void Update()
        //{
            //var emi = _particles.emission;
            //float amount = ComputeParticleEmission();
            //emi.rateOverTime = new ParticleSystem.MinMaxCurve(amount);
        //}

        //public void Stop()
        //{
        //    var emi = _particles.emission;
        //    emi.enabled = false;
        //}

        /// <summary>
        /// Computes the particle count according to the current angle of the Relative Transform set.
        /// </summary>
        /// <returns></returns>
        //private float ComputeParticleEmission()
        //{
        //    var angle = Vector3.Dot(transform.forward, Vector3.down);
        //    return angle * maxEmission;
        //}
    }
}