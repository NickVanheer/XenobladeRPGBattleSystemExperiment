using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisengageWhenDistanceBetweenEnemy : MonoBehaviour {

    private float distance;
	void Update () {

        if(GetComponent<RPGActor>().State == ActorState.Engaged && GetComponent<RPGActor>().TargetObject != null)
        {
            distance = GetComponent<RPGActor>().TargetObject.GetComponent<RPGActor>().DisengageDistance;
            float d = Mathf.Abs(Vector3.Distance(this.transform.position, GetComponent<RPGActor>().TargetObject.transform.position));

            if (d >= distance)
            {
                GetComponent<RPGActor>().TargetObject.GetComponent<RPGActor>().DisengageAllEngagedEnemies();
                foreach (var member in GameManager.Instance.GetPartyMembers())
                {
                    member.DisengageAllEngagedEnemies();
                }
            }
        }
    }
}
