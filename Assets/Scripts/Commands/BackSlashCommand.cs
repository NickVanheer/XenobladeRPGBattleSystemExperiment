using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackSlashCommand : Command
{
	void Start () {

        Name = LocalizationManager.Instance.GetLocalizedValue("skillBackslash_N");
        Description = LocalizationManager.Instance.GetLocalizedValue("skillBackslash_D");
        Slot = 3;
       Cooldown = 15;
       Illustration = "Skills/BackSlash";
       ActionVariable += () =>
        {
            if (GetComponent<RPGActor>().IsBehindTarget())
            {
                GetComponent<RPGActor>().DoDamageAttackOnCurrentTarget(18, true, 10);
                CoreUIManager.Instance.SpawnLabel("Back attack!", GetComponent<RPGActor>().TargetObject);
                ChainBarDisplayController.Instance.AddToChainBar(8);
            }
            else
            {
                GetComponent<RPGActor>().DoDamageAttackOnCurrentTarget(10);
                ChainBarDisplayController.Instance.AddToChainBar(4);
            }
        };

        ResetCommand();
        GetComponent<RPGActor>().PartyMemberCommands.Add(this);
    }
}
