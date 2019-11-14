using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflictBreakCommand : Command {

    void Start () {
        Cooldown = 15;
        Name = LocalizationManager.Instance.GetLocalizedValue("skillInflictBreak_N");
        ActionVariable += () =>
        {
            var enemies = GetComponent<RPGActor>().EngagedEnemies;
            RPGActor enemyToBreak = null;
            foreach (var enemy in enemies)
            {
                if (enemy.GetComponent<RPGActor>().Properties.IsBreak == false)
                    enemyToBreak = enemy.GetComponent<RPGActor>();
            }

            CoreUIManager.Instance.SpawnLabel(Name, this.gameObject);
            GetComponent<RPGActor>().InflictBreak(enemyToBreak);
        };

        ResetCommand();
        IsEnabled = true;
        GetComponent<RPGActor>().PartyMemberCommands.Add(this);
	}

    //Get realtime localized value when needed.
    public override string GetName()
    {
        return LocalizationManager.Instance.GetLocalizedValue("skillInflictBreak_N");
    }
}
