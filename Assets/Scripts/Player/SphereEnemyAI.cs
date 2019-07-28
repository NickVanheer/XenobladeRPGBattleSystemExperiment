﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereEnemyAI : BaseAI {

    public float CleaveAttackCooldown = 30f;
    private float cleaveTimer = 0;

    public float AoEAttackCooldown = 30f;
    private float aoeTimer = 0;

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
            actor.DoDamageAttackWithLabel(10, "Cleave", true);
        }

        aoeTimer += Time.deltaTime;
        if (aoeTimer >= AoEAttackCooldown)
        {
            aoeTimer = 0f;
            GameManager.Instance.SpawnAoE(this.gameObject, GameManager.Instance.GetPartyLeaderFloorObject().transform.position, 20, 2f, 3.0f);
        }
    }
}