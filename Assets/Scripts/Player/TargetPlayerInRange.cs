using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPlayerInRange : MonoBehaviour {

    public float Range = 150;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (GameManager.Instance.IsGameOver)
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
