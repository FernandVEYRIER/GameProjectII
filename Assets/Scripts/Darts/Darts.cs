using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using UnityEngine.Networking;

public class Darts : APlayerController
{

    private int round = 0;
    public GameObject verticalLine;
    public GameObject horizontalLine;
    private bool isLock = false;
    public GameObject dart;

    // Use this for initialization
    void Start()
    {
        verticalLine.SetActive(false);
    }

    /// <summary>
    /// Called when the local player is ready.
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        Debug.Log("Starting local player " + _playerColor + "   " + _playerName);
        base.OnStartLocalPlayer();
    }
	
    IEnumerator Unlock()
    {
        yield return new WaitForSeconds(1);
        isLock = false;
    }

    public void ThrowDart()
    {
        Instantiate(dart, new Vector3(verticalLine.transform.position.x, horizontalLine.transform.position.y, 5.253f), Quaternion.identity);
        
    }

    // Update is called once per frame
    void Update () {
        dart.GetComponent<MeshRenderer>().sharedMaterial.color = _playerColor;
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(verticalLine.activeSelf + " " + verticalLine.GetComponent<Line>().isMoving);
            //Touch anywhere
            if (round < 3 && !isLock)
            {
                if (!verticalLine.activeSelf)
                {
                    horizontalLine.GetComponent<Line>().isMoving = false;
                    verticalLine.SetActive(true);
                }
                else if (verticalLine.activeSelf && verticalLine.GetComponent<Line>().isMoving == true)
                {
                    verticalLine.GetComponent<Line>().isMoving = false;
                    isLock = true;
                    ++round;
                    ThrowDart();
                    StartCoroutine(Unlock());
                }
            }
        }
        if (!isLock && horizontalLine.GetComponent<Line>().isMoving == false && verticalLine.GetComponent<Line>().isMoving == false && round < 3)
        {
            horizontalLine.GetComponent<Line>().isMoving = true;
            verticalLine.GetComponent<Line>().isMoving = true;
            verticalLine.SetActive(false);
        }
    }
}
