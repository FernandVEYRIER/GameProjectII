using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.GoSoju
{
    public class GameUI : NetworkBehaviour
    {
        [SerializeField] Text counter;

        [Server]
        public void StartRpcCounter()
        {
            RpcCounter();
        }

        [ClientRpc]
        private void RpcCounter()
        {
            StartCoroutine("Counter");
        }

        private IEnumerator Counter()
        {
            counter.enabled = true;
            counter.text = "3";
            yield return new WaitForSeconds(1);
            counter.text = "2";
            yield return new WaitForSeconds(1);
            counter.text = "1";
            yield return new WaitForSeconds(1);
            counter.text = "GO SOJU !";
            yield return new WaitForSeconds(1);
            counter.enabled = false;
            yield return 0;
        }
    }
}
