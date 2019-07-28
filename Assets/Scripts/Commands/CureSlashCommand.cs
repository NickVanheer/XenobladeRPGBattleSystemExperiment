using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CureSlashCommand : Command {

    // Use this for initialization
    void Start () {

       Name = "Cure Slash";
       Description = "A slash that spawns a health potion.";
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
        IsEnabled = true;
        GetComponent<RPGActor>().PartyMemberCommands.Add(this);
    }
}
