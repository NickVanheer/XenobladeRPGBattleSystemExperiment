using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAI : BaseAI {
    public override void ResetOnEnterIdleState()
    {
    }

    public override void UpdateLoop()
    {
        //We're the leader so disable moving towards the enemy automatically when there's an AOE that we need to avoid.
        if (GameManager.Instance.ActiveAOEs > 0)
            this.CanFollowTarget = false;
        else
            this.CanFollowTarget = true;
    }
}
