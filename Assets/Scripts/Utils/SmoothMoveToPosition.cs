using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMoveToPosition : MonoBehaviour {

    // Use this for initialization
    public Transform Target;
    public bool IsMoving = false;
    public bool IsLoop = true;
    public float Speed;
    public Vector3 MoveOffset;

    Vector3 moveTarget;
    Vector3 startPosition;
    Vector3 endPosition;

    public void Start()
    {
        if (Target == null)
            moveTarget = this.transform.position + MoveOffset;
        else
            moveTarget = Target.position;

        startPosition = this.transform.position;
        endPosition = moveTarget;
    }

    // Activate the object with looping disabled to do one single move animation, and then disable itself. Reactivating it will play the second animation. (...)
    void Update () {
		if(IsMoving)
        {
            float step = Speed * Time.deltaTime;

            // Move our position a step closer to the target.
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, step);

            if(moveTarget != startPosition && Vector3.Distance(transform.position, endPosition) <= 0.1f)
            {
                moveTarget = startPosition;
                if (!IsLoop)
                    enabled = false;
            }
            else if (moveTarget != endPosition && Vector3.Distance(transform.position, startPosition) <= 0.1f)
            {
                moveTarget = endPosition;
                if (!IsLoop)
                    enabled = false;
            }
        }
	}
}
