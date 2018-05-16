﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Networking;

public class BottleScript : NetworkBehaviour {
    [SerializeField]
    private Color _color;

    public GameObject coaster = null;

    //[SyncVar(hook = "OnParentChange")]
    //public Vector3 _parent;

    [SerializeField]
    private float jumpSpeed = 3f;

    [SyncVar]
    public bool jumping = false;

    public bool falling = false;

    [HideInInspector]
    [SyncVar]
    public bool striking = false;

    [HideInInspector]
    [SyncVar]
    public bool faking = false;

    [SyncVar(hook = "OnStrikerChange")]
    public bool isStriker = false;

    public GameObject hammer = null;

    // Use this for initialization
    void Start() {
    }

    private void Awake() {
        RectTransform panelLoadingScreen = LobbyManager.Instance.panelLoading;
        panelLoadingScreen.gameObject.SetActive(false);
    }

    public override void OnStartAuthority() {
        this.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    // Update is called once per frame
    void Update() {
        if (localPlayerAuthority && !isStriker) {
            if (coaster != null)
                this.transform.parent = coaster.transform;
            if (hasAuthority) {
                Moving();
            }
        }
    }

    public void Moving() {
        if (localPlayerAuthority) {
//           Debug.Log("Moving " + jumping + " " + falling);
            Vector3 newPosition = this.transform.position;
            Vector3 tmpPosition = this.transform.position;

            tmpPosition.y -= GetComponent<Collider>().bounds.size.y / 2;
            if (jumping == true) {

                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(tmpPosition, -Vector3.up, out hit)) {
//                    Debug.Log("Jumping " + hit.distance + " " + this.name);
                    float distanceToGround = hit.distance;
                    if (distanceToGround > 1.5f) {
                        jumping = false;
                        falling = true;
                    } else {
                        newPosition.y += Time.deltaTime * jumpSpeed;
                    }
                }
            } else if (falling == true) {
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(tmpPosition, -Vector3.up, out hit)) {
//                    Debug.Log("Falling " + hit.distance);
                    float distanceToGround = hit.distance;

                    if (distanceToGround <= 0.1f) {
                        falling = false;
                        newPosition.y = coaster.transform.position.y + coaster.GetComponent<Collider>().bounds.size.y / 2 + GetComponent<Collider>().bounds.size.y / 2;
                    } else {
                        newPosition.y -= Time.deltaTime * jumpSpeed;
                    }
                }
            }

            // Detach Player From Coaster On Destruction
            if (this.coaster && this.coaster.GetComponent<CoasterScript>().striked && this.transform.parent.position.x > 20) {
                this.transform.parent = null;
                this.transform.position = new Vector3(100, 100, 100);
                this.coaster = null;
            }

            this.transform.position = newPosition;
        }
    }

    [Command]
    public void CmdFalling() {

    }

    [ClientRpc]
    public void RpcCoaster(GameObject coaster) {
        this.coaster = coaster;
    }

    [ClientRpc]
    public void RpcName(string name) {
        this.name = name;
    }

    public void OnStrikerChange(bool isStriker) {
        this.isStriker = isStriker;
    }
}
