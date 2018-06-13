using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game;
using Assets.Scripts.Networking;
using UnityEngine.Networking;
using Assets.Scripts.Darts;

public class Darts : APlayerController
{
    [SerializeField] private Transform _targetCenter;
    [SerializeField] private GameObject _verticalLinePrefab;
    [SerializeField] private GameObject _horizontalLinePrefab;
    [SerializeField] private Transform _limitPoint;

    private int round = 0;
    private GameObject verticalLine;
    private GameObject horizontalLine;
    private bool isLock = false;
    public GameObject dart;

    private float _score = 0;

    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer)
        {
            verticalLine = Instantiate(_verticalLinePrefab);
            horizontalLine = Instantiate(_horizontalLinePrefab);
            verticalLine.SetActive(false);
            _targetCenter = GameObject.FindGameObjectWithTag("Finish").GetComponent<Transform>();
            _limitPoint = GameObject.FindGameObjectWithTag("LimitPoint").GetComponent<Transform>();
        }
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
        float tmp;
        var pos = new Vector3(verticalLine.transform.position.x, horizontalLine.transform.position.y, 5.253f);
        Instantiate(dart, pos, Quaternion.identity);
        tmp = Mathf.Round(100 - Vector2.Distance(pos, _targetCenter.position) / Vector2.Distance(_limitPoint.position, _targetCenter.position) * 100);
        if (tmp < 0)
            tmp = 0;
        _score += tmp;
        Debug.Log("Result => " + _score);
        if (round == 3)
        {
            CmdUpdateScore(_score);
        }
    }

    [Command]
    private void CmdUpdateScore(float score)
    {
        Debug.Log("Server score => " + score);
        (AGameManager.Instance as GameManager).SetScore(_playerName, score);
    }

    // Update is called once per frame
    void Update () {
        if (isLocalPlayer)
        {
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
}
