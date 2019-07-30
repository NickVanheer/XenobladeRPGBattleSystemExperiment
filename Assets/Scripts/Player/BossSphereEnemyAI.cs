using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSphereEnemyAI : BaseAI {

    public int AoESize = 1;
    public int AoECooldown = 4;
    bool isSpecialUsed = false;
    float aoeTimer;

    public override void DefaultAttack()
    {
        if(aoeTimer > AoECooldown)
        {
            aoeTimer = 0f;

            int kind = Random.Range(0, 2);

            if(kind == 0)
                GameManager.Instance.SpawnAoE(this.gameObject, GameManager.Instance.GetPartyLeaderFloorObject().transform.position, 20, 4f, 4.0f);

            if(kind == 1)
                GameManager.Instance.SpawnTwoRectangleAoE(this.gameObject, GetFloorPosition(), 20, 3.0f);
        }

        if (actor.Properties.GetHealthPercentage() < 50 && !isSpecialUsed)
        {
            GameManager.Instance.SpawnAoE(this.gameObject, GetFloorPosition(), 60, 3f, 3.0f);
            CoreUIManager.Instance.SpawnLabel("Blaze", actor.gameObject);
            isSpecialUsed = true;
            return;
        }

        int skillDamage = Random.Range(9, 14);
        actor.DoDamageAttackOnCurrentTarget(skillDamage);
    }

    public override void ResetOnEnterIdleState()
    {
        isSpecialUsed = false;
    }

    public override void UpdateLoop()
    {
        aoeTimer += Time.deltaTime;
    }
}
