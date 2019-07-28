using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToppleHealCommand : Command {

    // Use this for initialization
    void Start () {
        Name = "Topple Heal";
        Description = "Inflicts Topple on broken enemies and spawns a health potion.";
        Slot = 1;
        IsInflictTopple = true;
        Cooldown = 30;
        Illustration = "Skills/AttackBoost";
        ActionVariable += () =>
        {
            GetComponent<RPGActor>().DoDamageAttackOnCurrentTarget(8);
            GetComponent<RPGActor>().InflictToppleOnTarget();
            GameManager.Instance.SpawnHealthPotion();
            ChainBarDisplayController.Instance.AddToChainBar(4);
        };
        ResetCommand();
        IsEnabled = true;
        GetComponent<RPGActor>().PartyMemberCommands.Add(this);
	}
}
