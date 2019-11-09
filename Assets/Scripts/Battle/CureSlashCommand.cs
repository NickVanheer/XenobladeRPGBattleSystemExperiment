using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CureSlashCommand : Command {

    // Use this for initialization
    void Start () {

        Name = GetName();
        Description = GetDescription();
        Slot = 0;
       Cost = 1;
       Cooldown = 40;
       Illustration = "Skills/SwordCure";
       ActionVariable += () =>
        {
            GetComponent<RPGActor>().DoDamageAttackOnCurrentTarget(10);
            GameManager.Instance.SpawnHealthPotion();
            ChainBarDisplayController.Instance.AddToChainBar(5);
        };

        ResetCommand();
        GetComponent<RPGActor>().PartyMemberCommands.Add(this);
    }

    //Get realtime localized value when needed.
    public override string GetName()
    {
        return LocalizationManager.Instance.GetLocalizedValue("skillCureslash_N");
    }

    public override string GetDescription()
    {
        return LocalizationManager.Instance.GetLocalizedValue("skillCureslash_D");
    }
}
