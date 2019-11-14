using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2WhiteMageAI : BaseAI {

    public override void DefaultAttack()
    {
        base.DefaultAttack();

        var healCommand = GetComponent<HealPartyMemberCommand>();
        if (healCommand != null && healCommand.CanExecute())
        {
            healCommand.UseCommand();
        }
    }

    public override void ResetOnEnterIdleState()
    {
    }

    public override void UpdateLoop()
    {
    }
}
