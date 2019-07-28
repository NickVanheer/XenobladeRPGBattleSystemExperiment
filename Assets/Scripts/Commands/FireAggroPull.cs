﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAggroPull : Command {

    void Start () {
        Name = "Fire Slash";
        Description = "A fire elemental slash that draws aggro.";
        Slot = 4;
        Cooldown = 35;
        Element = MagicElemancy.Fire;
        Illustration = "Skills/SwordFire";
        ActionVariable += () =>
        {
            GetComponent<RPGActor>().DoDamageAttackOnCurrentTarget(12, MagicElemancy.Fire, 0);
            ChainBarDisplayController.Instance.AddToChainBar(5);
            GetComponent<RPGActor>().Target.PullAggro(this.gameObject);
            //CoreUIManager.Instance.SpawnLabel("Fire!", GetComponent<RPGActor>().TargetObject); 
        };

        ResetCommand();
        IsEnabled = true;
        GetComponent<RPGActor>().PartyMemberCommands.Add(this);
    }
}