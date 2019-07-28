using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroCommand : Command {

    public AggroCommand()
    {
    }

    // Use this for initialization
    void Start () {
        Cooldown = 25;
        Name = "Aggro Slash";
        ActionVariable += () =>
        {
            RPGActor leader = GameManager.Instance.CurrentPartyMembers[0].GetComponent<RPGActor>();

            if (leader != null && leader.State == ActorState.Engaged)
            {
                CoreUIManager.Instance.SpawnLabel(Name, this.gameObject);
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
}
