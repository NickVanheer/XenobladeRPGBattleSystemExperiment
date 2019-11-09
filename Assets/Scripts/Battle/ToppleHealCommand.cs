using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToppleHealCommand : Command {

    // Use this for initialization
    void Start () {
        Name = GetName();
        Description = GetDescription();
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

    //Get realtime localized value when needed.
    public override string GetName()
    {
        return LocalizationManager.Instance.GetLocalizedValue("skillToppleHeal_N");
    }

    public override string GetDescription()
    {
        return LocalizationManager.Instance.GetLocalizedValue("skillToppleHeal_D");
    }
}

