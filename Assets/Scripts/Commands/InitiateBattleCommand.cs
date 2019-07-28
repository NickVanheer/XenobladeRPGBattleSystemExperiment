using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiateBattleCommand : Command {
    public InitiateBattleCommand()
    {
    }

    // Use this for initialization
    void Start () {

        Name = "Start";
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
        IsEnabled = true;
        GetComponent<RPGActor>().PartyMemberCommands.Add(this);

    }
}
