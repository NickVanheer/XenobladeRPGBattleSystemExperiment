using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPlayerInRange : MonoBehaviour {

    public float Range = 150;

	void Update () {

        if (GameManager.Instance.IsGameOver || GameManager.Instance.GetLeaderActor().State == ActorState.Dead)
            return;

        RPGActor enemyActor = GetComponent<RPGActor>();
        if(enemyActor.State == ActorState.Idle)
        {
            float distance = Vector3.Distance(this.transform.position, GameManager.Instance.GetPartyLeader().transform.position);
            if (Mathf.Abs(distance) < Range)
            {
                enemyActor.SetTarget(GameManager.Instance.GetPartyLeader());
                enemyActor.EngageTarget(); //notify the enemy too.
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
