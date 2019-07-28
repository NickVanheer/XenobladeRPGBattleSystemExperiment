using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveDeadPartyMemberWhenOutOfCombat : MonoBehaviour {

	// Update is called once per frame
	void Update () {

        RPGActor actor = GetComponent<RPGActor>();
        bool isOutOfCombat = GameManager.Instance.CurrentState == GameManager.Instance.StateIdle;

        if(actor.State == ActorState.Dead && isOutOfCombat)
        {
            actor.EnterIdleState();
            actor.RestoreHP(50, true);
        }
    }
}
