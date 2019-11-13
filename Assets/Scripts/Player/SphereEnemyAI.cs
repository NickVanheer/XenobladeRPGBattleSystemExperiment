using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereEnemyAI : BaseAI {

    public float CleaveAttackCooldown = 30f;
    public float AoEAttackCooldown = 30f;
    public bool SpawnFallingAoE = false;

    private float aoeTimer = 0;
    private float cleaveTimer = 0;

    public override void DefaultAttack()
    {
        int dmg = Random.Range(3, 6);
        actor.DoDamageAttackOnCurrentTarget(dmg);
    }

    public override void ResetOnEnterIdleState()
    {
    }

    public override void UpdateLoop()
    {
        cleaveTimer += Time.deltaTime;
        if (cleaveTimer >= CleaveAttackCooldown)
        {
            cleaveTimer = 0f;
            actor.DoDamageAttackWithLabel(10, "(。-`ω´-)", true);
        }

        aoeTimer += Time.deltaTime;
        if (aoeTimer >= AoEAttackCooldown)
        {
            aoeTimer = 0f;

            if(SpawnFallingAoE)
            {
                int r = Random.Range(1, 10);

                if (r > 5)
                    GameManager.Instance.SpawnFallingAoE(this.gameObject, Vector3.zero, 20, 1f, 3.0f);
            }
            else
            {
                GameManager.Instance.SpawnAoE(this.gameObject, GameManager.Instance.GetPartyLeaderFloorObject().transform.position, 20, 2f, 3.0f);
            }
        }
    }
}
