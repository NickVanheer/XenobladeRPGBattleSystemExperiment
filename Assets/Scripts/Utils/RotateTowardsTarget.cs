using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsTarget : MonoBehaviour {

    public GameObject Target;
	
	void Update () {

       Target = GetComponent<RPGActor>().TargetObject;

        if (Target != null)
        {
            //transform.LookAt(Target.transform.position, Vector3.back);
            var targetRotation = Quaternion.LookRotation(Target.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
    }
}
