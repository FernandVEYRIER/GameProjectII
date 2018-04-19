using UnityEngine;

namespace Assets.Scripts.CantRoachThis
{
    public class SwatterCollider : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("On trigger enter");
            if (other.tag.Equals("Player"))
            {
                Destroy(other.gameObject);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("On collision enter");
            if (collision.transform.tag.Equals("Player"))
            {
                Destroy(collision.gameObject);
            }
        }
    }
}