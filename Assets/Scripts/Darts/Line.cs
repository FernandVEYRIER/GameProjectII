using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

    public bool isVertical;
    public bool isMoving = true;
    public GameObject mini;
    public GameObject maxi;
    public float speed;
    private int side = 1;

    // Use this for initialization
    private void Start () {
        if (isVertical)
        {
            gameObject.transform.position = new Vector3(Random.Range(mini.transform.position.x, maxi.transform.position.x), transform.position.y, transform.position.z);
        }
        else
        {
            gameObject.transform.position = new Vector3(transform.position.x, Random.Range(mini.transform.position.y, maxi.transform.position.y), transform.position.z);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (isMoving)
		    if (isVertical)
            {
                transform.Translate(Vector3.right * speed * Time.deltaTime * side, Space.World);
                if ((side > 0 && gameObject.transform.position.x >= maxi.transform.position.x) || (side < 0 && gameObject.transform.position.x <= mini.transform.position.x))
                    side *= -1;
            }
            else
            {
                transform.Translate(new Vector3(0.0f, 1.0f, 0.0f) * speed * Time.deltaTime * side, Space.World);
                if ((side > 0 && gameObject.transform.position.y >= maxi.transform.position.y) || (side < 0 && gameObject.transform.position.y <= mini.transform.position.y))
                    side *= -1;
            }
	}
}
