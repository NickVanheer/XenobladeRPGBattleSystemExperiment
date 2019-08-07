using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflictBreakCommand : Command {

    void Start () {
        Cooldown = 15;
        Name = LocalizationManager.Instance.GetLocalizedValue("skillInflictBreak_N");
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
