using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAggroPull : Command {

    void Start () {
        Name = GetName();
        Description = GetDescription();
        Slot = 4;
        Cooldown = 35;
        Element = MagicElemancy.Fire;
        Illustration = "Skills/SwordFire";
        ActionVariable += () =>
        {
            GetComponent<RPGActor>().DoDamageAttackOnCurrentTarget(12, MagicElemancy.Fire, 0);
            ChainBarDisplayController.Instance.AddToChainBar(5);
            GetComponent<RPGActor>().Target.PullAggro(this.gameObject);
        };

        ResetCommand();
        GetComponent<RPGActor>().PartyMemberCommands.Add(this);
    }

    //Get realtime localized value when needed.
    public override string GetName()
    {
        return LocalizationManager.Instance.GetLocalizedValue("skillFireAggroPull_N");
    }

    public override string GetDescription()
    {
        return LocalizationManager.Instance.GetLocalizedValue("skillFireAggroPull_D");
    }
}
