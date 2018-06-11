using System.Collections;
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

    [SyncVar]
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

    private float marge = 0f;

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
            Vector3 newPosition = this.transform.position;
            Vector3 tmpPosition = this.transform.position;

            tmpPosition.y -= GetComponent<Collider>().bounds.size.y / 2;
            if (jumping == true) {
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(tmpPosition, -Vector3.up, out hit)) {
                    float distanceToGround = hit.distance;

                    if (distanceToGround > 1.5f) {
                        this.jumping = false;
                        CmdJumping(false);
                        CmdFalling(true);
                    } else {
                        newPosition.y += Time.deltaTime * jumpSpeed;
                    }
                }
            } else if (falling == true) {
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(tmpPosition, -Vector3.up, out hit)) {
                    float distanceToGround = hit.distance;

                    if (distanceToGround <= 0.1f) {
                        this.falling = false;
                        CmdFalling(false);
                        if (coaster)
                            newPosition.y = coaster.transform.position.y + coaster.GetComponent<Collider>().bounds.size.y / 2 + GetComponent<Collider>().bounds.size.y / 2;
                        else
                            newPosition.y = 0.5f;
                    } else {
                        newPosition.y -= Time.deltaTime * jumpSpeed;
                    }
                }
            }

            this.transform.position = newPosition;

            // Detach Player From Coaster On Destruction
            if (this.coaster && this.coaster.GetComponent<CoasterScript>().striked && this.transform.parent.position.x > 20) {
                CmdSetParent();
                this.transform.parent = null;
                this.transform.position = new Vector3(marge - 5, 0, 10);
                this.coaster = null;
            }
        }
    }

    [Command]
    public void CmdSetParent() {
        if (isServer) {
            this.transform.parent = null;
            this.coaster = null;
            RpcSetParent();
        }
    }

    [ClientRpc]
    public void RpcSetParent() {
        if (localPlayerAuthority) {
            this.transform.parent = null;
            this.coaster = null;
        }
    }

    [Command]
    public void CmdJumping(bool jumping) {
        if (isServer) {
            this.jumping = jumping;
        }
    }

    [Command]
    public void CmdFalling(bool falling) {
        if (isServer) {
            this.falling = falling;
        }

    }

    [ClientRpc]
    public void RpcCoaster(GameObject coaster) {
        if (coaster == null) {
            CmdSetParent();
            this.transform.position = Vector3.zero;
        } else {
            this.coaster = coaster;
        }
    }

    [ClientRpc]
    public void RpcName(string name) {
        this.name = name;
    }

    [ClientRpc]
    public void RpcMarge(float marge) {
        this.marge = marge;
    }

    public void OnStrikerChange(bool isStriker) {
        this.isStriker = isStriker;
    }
}
