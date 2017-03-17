using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colliderControl : MonoBehaviour {
    BoxCollider collider;
	// Use this for initialization
	void Start () {
        collider=GetComponent<BoxCollider>();
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.parent.GetComponent<Canvas>().enabled == false)
        {
            collider.enabled = false;
        }
        else
        {
            collider.enabled = true;
        }
	}
}
