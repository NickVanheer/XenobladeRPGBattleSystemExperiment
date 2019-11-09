using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPositionToGameObject : MonoBehaviour {

    public GameObject Target;
    public bool IgnoreY = true;
	
	// Update is called once per frame
	void Update () {

        if (Target == null)
            return;

        Vector3 pos = Target.transform.position;
        pos.y = this.transform.position.y;

        this.transform.position = pos;
	}
}
