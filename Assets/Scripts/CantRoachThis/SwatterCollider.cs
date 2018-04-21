using UnityEngine;

namespace Assets.Scripts.CantRoachThis
{
    public delegate void EventTrigger(object caller, Collider other);

    /// <summary>
    /// Handles collisions with the swatter.
    /// </summary>
    public class SwatterCollider : MonoBehaviour
    {
        public event EventTrigger OnTriggerEntered;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("On trigger enter");
            if (OnTriggerEntered != null)
                OnTriggerEntered.Invoke(this, other);
        }
    }
}