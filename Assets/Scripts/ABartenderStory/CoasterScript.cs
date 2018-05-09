using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CoasterScript : NetworkBehaviour {

    [SyncVar(hook = "OnColorChange")] public int ObjectColor = 1;
    [SyncVar(hook = "OnMainCoasterChange")] public bool mainCoaster = false;
    public float speed = 15f;
    public Color[] colors = null;
    [SyncVar(hook = "OnStrikeChange")] public bool striked = false;
    private Vector3 position = new Vector3();

	// Use this for initialization
	void Start () {
        position = this.transform.position;
	}

    public override void OnStartClient() {
        base.OnStartClient();
    }

    public override void OnStartLocalPlayer() {
        GetComponent<Renderer>().material.color = Color.blue;
    }

    // Update is called once per frame
    void Update () {
        if (localPlayerAuthority) {
            if (striked == true) {
                if (transform.position.x < 200) {
                    position.x += speed * Time.deltaTime;
                    transform.position = position;
                } else {
                    Destroy(this.gameObject);
                }
            }
        }
	}

    public void SetStriked(bool striked) {
        this.striked = striked;
    }

    private void OnColorChange(int newColor) {
        if (localPlayerAuthority) {
            this.GetComponent<Renderer>().material.color = colors[newColor];
        }
    }

    private void OnMainCoasterChange(bool isMainCoaster) {
        if (localPlayerAuthority) {
            mainCoaster = isMainCoaster;
        }
    }

    private void OnStrikeChange(bool isStriked) {
        if (localPlayerAuthority) {
            this.striked = isStriked;
        }
    }
}
