using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public GameObject Target;
    public GameObject Player;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 toTarget = (Target.transform.position - transform.position).normalized;

        float dt = Vector3.Dot(toTarget, Target.transform.forward);
        //Debug.Log(dt);

        if (dt < 0)
            Debug.Log("Front");
        else if (dt > 0)
            Debug.Log("Back");


        //Debug.DrawRay(Target.transform.position, Target.transform.forward * 20, Color.red);
        //Debug.DrawRay(transform.position, toTarget * 20, Color.green);

    }
}
