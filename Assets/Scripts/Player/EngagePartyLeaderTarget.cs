using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngagePartyLeaderTarget : MonoBehaviour {

	void Update () {

        if (this.GetComponent<RPGActor>().State == ActorState.Dead)
            return;

        RPGActor leader = GameManager.Instance.GetPartyLeader().GetComponent<RPGActor>();

        //Engage when party leader target has changed
        if (leader != null && leader.State == ActorState.Engaged)
        {
            if (GetComponent<RPGActor>().TargetObject != leader.TargetObject)
            {
                GameManager.Instance.Log("Actor " + this.name + " now starts engaging party leader target");
                GetComponent<RPGActor>().SetTarget(leader.TargetObject);
                GetComponent<RPGActor>().EngageTarget();
            }
        }

        //drop target when party leader is not engaged.
        if (leader != null && leader.State == ActorState.Idle && GetComponent<RPGActor>().State == ActorState.Engaged)
        {
            GetComponent<RPGActor>().DisengageAllEngagedEnemies();
        }




    }
}
