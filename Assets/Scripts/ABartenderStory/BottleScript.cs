using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Assets.Scripts.Networking;

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

    [HideInInspector]
    [SyncVar]
    public bool striking = false;

    [SyncVar(hook = "OnStrikerChange")]
    public bool isStriker = false;

    public GameObject hammer = null;

	// Use this for initialization
	void Start () {
    }

    private void Awake() {
        RectTransform panelLoadingScreen = LobbyManager.Instance.panelLoading;
        panelLoadingScreen.gameObject.SetActive(false);
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

            // Detach Player From Coaster On Destruction
            if (this.coaster && this.coaster.GetComponent<CoasterScript>().striked && this.transform.parent.position.x > 20) {
                this.transform.parent = null;
                this.transform.position = new Vector3(100, 100, 100);
                this.coaster = null;
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
