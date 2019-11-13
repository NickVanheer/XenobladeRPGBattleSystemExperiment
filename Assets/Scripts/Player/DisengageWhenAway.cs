using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisengageWhenAway : MonoBehaviour {

    public float MoveSpeed = 5;
    public float Range = 50;
    private Vector3 startingPosition;
    private bool isMovingTowardsStartingPosition = false;

	void Start () {
        startingPosition = this.transform.position;	
	}
	
	void Update () {

        //When we're idle, we want to move to the original location when we can!
        if(GetComponent<RPGActor>().State == ActorState.Idle)
        {
            if (isMovingTowardsStartingPosition)
            {
                //rotation
                var targetRotation = Quaternion.LookRotation(startingPosition - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);

                //move to
                transform.position += transform.forward * MoveSpeed * Time.deltaTime;

                float d = Mathf.Abs(Vector3.Distance(this.transform.position, startingPosition));

                if (d <= 2)
                {
                    //Debug.Log($"Actor {this.gameObject.name} returned to starting position");
                    Debug.Log("Actor " + this.gameObject.name + " returned to starting position");
                    isMovingTowardsStartingPosition = false;
                }
            }
        }

        //When we're in combat, we want to disengage when straying too far
        if (GetComponent<RPGActor>().State == ActorState.Engaged)
        {
            //Disengage enemy from everyone that was fighting it when it's straying too far from its original location. 
            float dist = Vector3.Distance(this.transform.position, startingPosition);

            if (dist > Range)
            {
                isMovingTowardsStartingPosition = true;
                GetComponent<RPGActor>().DisengageAllEngagedEnemies();

                foreach (var member in GameManager.Instance.GetPartyMembers())
                {
                    member.Disengage(this.gameObject);
                }
            }
        }
		
	}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
