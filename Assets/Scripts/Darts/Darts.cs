using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Darts : MonoBehaviour {

    private int round = 0;
    public GameObject verticalLine;
    public GameObject horizontalLine;
    private bool isLock = false;

    // Use this for initialization
    void Start () {
        verticalLine.SetActive(false);
    }
	
    IEnumerator Unlock()
    {
        yield return new WaitForSeconds(1);
        isLock = false;
    }

    // Update is called once per frame
    void Update () {
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
