using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithTarget : MonoBehaviour {

    public GameObject Target;
    public bool IsIgnoreY = true;

	// Update is called once per frame
	void Update () {
        if (Target != null)
        {
            this.transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y, Target.transform.position.z);
            //this.transform.localScale *= Target.transform.localScale.x;
        }
	}
}
