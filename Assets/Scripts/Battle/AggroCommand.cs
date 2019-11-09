using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroCommand : Command {

    void Start () {
        Cooldown = 25;
        Name = GetName();
        ActionVariable += () =>
        {
            RPGActor leader = GameManager.Instance.CurrentPartyMembers[0].GetComponent<RPGActor>();

            if (leader != null && leader.State == ActorState.Engaged)
            {
                CoreUIManager.Instance.SpawnLabel(GetName(), this.gameObject);
                GetComponent<RPGActor>().Target.PullAggro(this.gameObject);
            }
        };

        ResetCommand();
        GetComponent<RPGActor>().PartyMemberCommands.Add(this);
	}

    public override void ResetCommand()
    {
        base.ResetCommand();
        StartCooldown = 10;
        CurrentCooldown = 10;
    }

    //Get realtime localized value when needed.
    public override string GetName()
    {
        return LocalizationManager.Instance.GetLocalizedValue("skillAggroPull_N");
    }
}
