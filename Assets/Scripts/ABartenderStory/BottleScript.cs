using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BottleScript : NetworkBehaviour {
    [SerializeField]
    private Color _color;

    public GameObject coaster = null;

    [SyncVar(hook = "OnParentChange")]
    public Vector3 _parent;

    [SerializeField]
    private float jumpSpeed = 3f;

    [HideInInspector]
    [SyncVar(hook = "OnJumpingChange")]
    public bool jumping = false;
    [HideInInspector]
    public bool falling = false;

    [SyncVar(hook = "OnStrikerChange")]
    public bool isStriker = false;

	// Use this for initialization
	void Start () {
    }

    public override void OnStartAuthority() {
        this.GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    // Update is called once per frame
    void Update () {
        if (localPlayerAuthority && !isStriker) {
            if (coaster != null)
                this.transform.parent = coaster.transform;

            Vector3 newPosition = this.transform.position;

            if (jumping == true) {
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(transform.position, -Vector3.up, out hit)) {
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

                if (Physics.Raycast(transform.position, -Vector3.up, out hit)) {
                    float distanceToGround = hit.distance;
                    if (distanceToGround <= 0.1f) {
                        falling = false;
                        newPosition.y = _parent.y;
                    } else {
                        newPosition.y -= Time.deltaTime * jumpSpeed;
                    }
                }
            }
            this.transform.position = newPosition;
        }
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

    public void OnParentChange(Vector3 newParent) {
        _parent = newParent;
        this.transform.position = _parent;
        Debug.Log(_parent);
    }

    public void OnJumpingChange(bool jumping) {
        this.jumping = jumping;
    }
}
