using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2AI : BaseAI {

    public override void DefaultAttack()
    {
        base.DefaultAttack();

        if (actor.PartyMemberCommands.Count == 0)
            return;

        //Use aggro and break commands when ready
        foreach (var attack in actor.PartyMemberCommands)
        {
            if (attack.CanExecute())
                attack.UseCommand();
        }
    }

    public override void ResetOnEnterIdleState()
    {
    }

    public override void UpdateLoop()
    {
    }
}
