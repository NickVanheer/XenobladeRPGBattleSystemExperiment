using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotationY : MonoBehaviour {

    Quaternion original;
	// Use this for initialization
	void Start () {
        original = this.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.rotation = original;
	}
}
