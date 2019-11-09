using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiateBattleCommand : Command {
    public InitiateBattleCommand()
    {
    }

    // Use this for initialization
    void Start () {

        Name = LocalizationManager.Instance.GetLocalizedValue("skillStart_N");
        Description = LocalizationManager.Instance.GetLocalizedValue("skillStart_D");
        Slot = 2;
        Cooldown = 0;
        Illustration = "Skills/SwordIcon";
        ActionVariable = () =>
        {
            GetComponent<RPGActor>().TargetSoftTarget();
            GetComponent<RPGActor>().EngageTarget(true);

            GameManager.Instance.EnterBattleState();
        };

        ResetCommand();

        IsAlwaysActiveCommand = true;
        GetComponent<RPGActor>().PartyMemberCommands.Add(this);
    }

    //Get realtime localized value when needed.
    public override string GetName()
    {
        return LocalizationManager.Instance.GetLocalizedValue("skillStart_N");
    }

    public override string GetDescription()
    {
        return LocalizationManager.Instance.GetLocalizedValue("skillStart_D");
    }
}
