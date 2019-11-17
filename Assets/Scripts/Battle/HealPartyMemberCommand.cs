using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealPartyMemberCommand : Command {

    void Start () {
        Cooldown = Random.Range(6,10);
        Name = LocalizationManager.Instance.GetLocalizedValue("whiteMage_N");
        ActionVariable += () =>
        {
            var partyMembers = GameManager.Instance.GetPartyMembers();
            RPGActor weakestUnit = this.GetComponent<RPGActor>();
            float lowestHealth = 99999999;

            foreach (var player in partyMembers.Where(p => p.State != ActorState.Dead))
            {
                if (player.Properties.CurrentHealth < lowestHealth) 
                {
                    lowestHealth = player.Properties.CurrentHealth;
                    weakestUnit = player;
                }
            }

            if(weakestUnit.Properties.CurrentHealth < weakestUnit.Properties.MaxHealth)
                weakestUnit.RestoreHP(30, true);
        };

        ResetCommand();
        IsEnabled = true;
        GetComponent<RPGActor>().PartyMemberCommands.Add(this);
	}
}
