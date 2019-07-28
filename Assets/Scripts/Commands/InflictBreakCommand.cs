using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflictBreakCommand : Command {

    public InflictBreakCommand()
    {
    }

    // Use this for initialization
    void Start () {
        Cooldown = 15;
        Name = "Break Slash";
        ActionVariable += () =>
        {
            RPGActor leader = GameManager.Instance.CurrentPartyMembers[0].GetComponent<RPGActor>();

            if (leader != null && leader.State == ActorState.Engaged)
            {
                CoreUIManager.Instance.SpawnLabel(Name, this.gameObject);
                GetComponent<RPGActor>().InflictBreakOnTarget();
            }
        };

        ResetCommand();
        IsEnabled = true;
        GetComponent<RPGActor>().PartyMemberCommands.Add(this);
	}
}
