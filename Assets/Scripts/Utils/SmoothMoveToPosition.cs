using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMoveToPosition : MonoBehaviour {

    // Use this for initialization
    public Transform Target;
    public bool IsMoving = false;
    public float Speed;
    public Vector3 MoveOffset;

    Vector3 moveTarget;
    Vector3 startPosition;
    Vector3 endPosition;

    public void Start()
    {
        if (Target == null)
        {
            moveTarget = this.transform.position + MoveOffset;
        }
        else
        {
            moveTarget = Target.position;
        }

        startPosition = this.transform.position;
        endPosition = moveTarget;
    }

    // Update is called once per frame
    void Update () {
		if(IsMoving)
        {
            float step = Speed * Time.deltaTime;

            // Move our position a step closer to the target.
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, step);

            if(Vector3.Distance(transform.position, endPosition) <= 0.1f)
                moveTarget = startPosition;
            else if (Vector3.Distance(transform.position, startPosition) <= 0.1f)
                moveTarget = endPosition;
        }
	}
}
