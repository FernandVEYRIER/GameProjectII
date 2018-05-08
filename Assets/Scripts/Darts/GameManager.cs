using Assets.Scripts.Networking;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Darts
{
    public class GameManager : NetworkBehaviour
    {
        public GameObject VerticalLine;
        public GameObject HorizontalLine;
        public float speed;
        private int round = 0;

        private void Start()
        {
            VerticalLine.gameObject.transform.position = new Vector3(Random.Range(-4.5f, 4.5f), 0f, 0f);
            HorizontalLine.gameObject.transform.position = new Vector3(0f, Random.Range(-6.5f, 6.5f), 0f);
            HorizontalLine.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (round >= 3)
            {

            }
            else if (!HorizontalLine.gameObject.activeSelf)
            {

            }
            else
            {

            }
        }
    }
}