using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMeshOnPlay : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.enabled = false;
	}
}
