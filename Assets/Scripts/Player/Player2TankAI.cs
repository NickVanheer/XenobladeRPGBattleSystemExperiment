using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2TankAI : BaseAI {

    public override void DefaultAttack()
    {
        base.DefaultAttack();

        if (actor.PartyMemberCommands.Count == 0)
            return;

        InflictBreakCommand breakCommand = GetComponent<InflictBreakCommand>();

        if(breakCommand != null && breakCommand.CanExecute())
            breakCommand.UseCommand();

        AggroCommand aggroCommand = GetComponent<AggroCommand>();

        if (aggroCommand != null && aggroCommand.CanExecute())
            aggroCommand.UseCommand();
    }

    public override void ResetOnEnterIdleState()
    {
    }

    public override void UpdateLoop()
    {
    }
}
