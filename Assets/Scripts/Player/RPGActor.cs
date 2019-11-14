using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public enum ActorState { Idle, Engaged, Dead };

public class RPGActor : MonoBehaviour {

    public int EngagedEnemiesCount;
    public RPGProperties Properties;

    [Space]
    public List<Command> PartyMemberCommands = new List<Command>();

    public float DisengageDistance = 90;
    public UnityEvent EnterIdleStateCallBack;
    public UnityEvent EnterEngageStateCallBack;
    public UnityEvent OnDestroyCallBack;

    public ActorState State { get; private set; }
    public string StateDebugString;

    public bool IsDebugDeathTest = false;

    [SerializeField]
    private bool isPlayer
    {
        get
        {
            return this.tag == "Player";
        }
    }

    public GameObject TargetObject { get; private set; }
    public RPGActor Target
    {
        get
        {
            if (TargetObject != null)
                return TargetObject.GetComponent<RPGActor>();
            else
                return null;
        }
    }

    public GameObject SoftTargetObject { get; private set; }
    public RPGActor SoftTarget
    {
        get
        {
            if (SoftTargetObject != null)
                return SoftTargetObject.GetComponent<RPGActor>();
            else
                return null;
        }
    }

    public GameObject ParticleSpawnPoint;

    public List<GameObject> EngagedEnemies = new List<GameObject>();
    public GameObject previousTarget;

    public bool IsLeader = false;

    private bool isExpAwarded = false;

    private void Start()
    {
        Reset();
    }

    public bool IsPlayer()
    {
        return this.gameObject.tag == "Player";
    }

    void Update () {

        if (IsDebugDeathTest)
            State = ActorState.Dead;

        StateDebugString = State.ToString();
        EngagedEnemiesCount = EngagedEnemies.Count;

        //Handle death
        if (Properties.CurrentHealth <= 0 && State != ActorState.Dead)
        {
            EnterDeathState();

            if(isPlayer && IsLeader)
            {
                //Do things that need to happen when the party leader dies
                CoreUIManager.Instance.HideSkillDisplay();
            }

            return;
        }

        if(Properties.IsBreak)
        {
            Properties.CurrentBreakDuration -= Time.deltaTime;

            if (Properties.CurrentBreakDuration <= 0)
                Properties.RemoveBreak();
        }

        if (Properties.IsTopple)
        {
            Properties.CurrentToppleDuration -= Time.deltaTime;

            if (Properties.CurrentToppleDuration <= 0)
                Properties.RemoveTopple();
        }

        //auto attack is handled in BaseAI or a separate component on an actor.
        if (State == ActorState.Engaged)
        {
            CheckForDeathTargets();

            //target has died, assign new one or exit state
            if (Target == null && EngagedEnemies.Count > 0)
                    SetTarget(EngagedEnemies[0]); //DO engaged enemies count in gamemanager

        }
	}

    public void CheckForDeathTargets()
    {
        if (IsLeader && EngagedEnemies.Count == 0)
        {
            GetComponent<PlayerTargetNearest>().ClearTargetSelection();

            var pMembers = GameManager.Instance.GetPartyMembers();
            foreach (var member in pMembers)
            {
  
                if (member.Properties.CurrentHealth <= 0)
                    member.Properties.CurrentHealth = 1;

                member.EnterIdleState();
            }

            return;
        }
        else if (EngagedEnemies.Count == 0)
        {
            EnterIdleState();
            return;
        }

        //check for death enemies
        for (int i = EngagedEnemies.Count - 1; i >= 0; i--)
        {
            if (EngagedEnemies[i] == null)
            {
                EngagedEnemies.RemoveAt(i);
            }
            else if (EngagedEnemies[i].GetComponent<RPGActor>().State == ActorState.Dead)
            {
                //add experience and destroy enemies when we're a player, use isExpAwarded to ensure we only do it once
                if (this.tag == "Player" && EngagedEnemies[i].GetComponent<RPGActor>().isExpAwarded == false)
                {
                    AddExperience(EngagedEnemies[i]);
                    EngagedEnemies[i].GetComponent<RPGActor>().EnterDeathState(); //calls OnDestroyCallback
                    EngagedEnemies[i].GetComponent<RPGActor>().isExpAwarded = true;
                    Destroy(EngagedEnemies[i]); //we destroy the enemy model too
                }

                EngagedEnemies.RemoveAt(i);
            }
        }
    }

    public void SpawnDamageParticles(MagicElemancy element)
    {
        if (ParticleSpawnPoint == null)
            return; 

        var muzzleVFX = Instantiate(CoreUIManager.Instance.ParticleMuzzle, ParticleSpawnPoint.transform.position, Quaternion.identity);

        switch (element)
        {
            case MagicElemancy.None:
                Instantiate(CoreUIManager.Instance.ParticleBounce, ParticleSpawnPoint.transform.position, Quaternion.identity);
                break;
            case MagicElemancy.Fire:
                Instantiate(CoreUIManager.Instance.ParticleFire, ParticleSpawnPoint.transform.position, Quaternion.identity);
                break;
            case MagicElemancy.Water:
                break;
            case MagicElemancy.Lighting:
                break;
            case MagicElemancy.Blizzard:
                break;
            default:
                break;
        }

        var muzzleParticle = muzzleVFX.GetComponent<ParticleSystem>();
        if (muzzleParticle != null)
            Destroy(muzzleVFX, muzzleParticle.main.duration);
    }

    //on target
    public void InflictBreakOnTarget()
    {
        Target.Properties.InflictBreak();
        CoreUIManager.Instance.SpawnLabel(LocalizationManager.Instance.GetLocalizedValue("Break"), TargetObject, "Textures/Break_Icon");
    }

    //on custom object
    public void InflictBreak(RPGActor target)
    {
        target.Properties.InflictBreak();
        CoreUIManager.Instance.SpawnLabel(LocalizationManager.Instance.GetLocalizedValue("Break"), target.gameObject, "Textures/Break_Icon");
    }

    //on target
    public void InflictToppleOnTarget()
    {
        bool toppled = Target.Properties.InflictTopple();

        if(toppled)
            CoreUIManager.Instance.SpawnLabel(LocalizationManager.Instance.GetLocalizedValue("Topple"), TargetObject, "Textures/Topple_Icon");
    }

    public void DoDamageAttackOnRandomTarget(int skillDamage, bool isSpecialDamage = false)
    {
        if (EngagedEnemies.Count == 0)
            return;

        int index = Random.Range(0, EngagedEnemies.Count - 1);
        GameObject targetObject = EngagedEnemies[index];

        DoDamageAttack(skillDamage, MagicElemancy.None, targetObject, 10f, true);
    }

    public void DoDamageAttackWithLabel(int skillDamage, string labelText, bool isRedLabel = false)
    {
        DoDamageAttackOnCurrentTarget(skillDamage);
        CoreUIManager.Instance.SpawnLabel(labelText, TargetObject, isRedLabel);
    }

    public void DoDamageAttackOnCurrentTarget(int skillDamage, MagicElemancy element, float fontIncrement = 0)
    {
        DoDamageAttack(skillDamage, element, TargetObject);
    }
    public void DoDamageAttackOnCurrentTarget(int skillDamage, bool isSpecialDamage = false, float fontIncrement = 0)
    {
        DoDamageAttack(skillDamage, MagicElemancy.None, TargetObject, fontIncrement, isSpecialDamage);
    }

    public void DoDamageAttack(int damage, GameObject target, float fontIncrement = 0)
    {
        DoDamageAttack(damage, MagicElemancy.None, target, 0);
    }

    public void DoDamageAttack(int damage, MagicElemancy element, GameObject target, float fontIncrement = 0, bool isSpecialDamage = false)
    {
        if (IsPlayer())
            ChainBarDisplayController.Instance.AddToChainBar(1.5f);

        RPGActor targetActor = target.GetComponent<RPGActor>();
        TargetObject = target;

        //If target is not engaged, engage it with this actor
        if (targetActor.State == ActorState.Idle)
        {
            targetActor.SetTarget(this.gameObject);
            targetActor.EngageTarget(); //notify the enemy too.

            //Surprise attack, also set the soft target and maybe do more later?
            if (TargetObject.tag == "Player" && GameManager.Instance.CurrentState == GameManager.Instance.StateIdle)
            {
                targetActor.SetSoftTarget(this.gameObject);
                GameManager.Instance.EnterBattleState();

                GameManager.Instance.Log("Attacked target is a player who was idle, now entering battle state and showing UI");
            }
        }

        var playerShoot = GetComponent<PlayerShoot>();
        if (playerShoot != null && target != null)
        {
            //playerShoot.TargetObject = target;
            playerShoot.ShootBullet(target);
        }

        DamageInfo damageInfo = ResolveDamageUnitAGivesUnitB(this.Properties, targetActor.Properties, damage);
        int dmg = (int)damageInfo.TotalDamage;
        targetActor.Properties.ChangeHealth(-dmg);
        targetActor.SpawnDamageParticles(element);

        if(fontIncrement == 0)
        {
            fontIncrement = damageInfo.CritMultiplier * 1.3f;

            //for auto attack size variation
            if (damage == 1)
                fontIncrement *= 0.6f;
            if (damage == 2)
                fontIncrement *= 0.9f;
            if (damage == 3)
                fontIncrement *= 1.1f;

            if (damage > 5)
                fontIncrement *= 1.6f;
        }

        Color playerDMG = CoreUIManager.Instance.PlayerTakeDamageColor;
        Color enemyDMG = CoreUIManager.Instance.EnemyTakeDamageColor;
        Color specialDMG = CoreUIManager.Instance.SpecialDamageColor;

        if(isSpecialDamage)
            CoreUIManager.Instance.SpawnText(dmg.ToString(), target, specialDMG, 2.6f);
        else if (this.tag == "Enemy")
            CoreUIManager.Instance.SpawnText(dmg.ToString(), target, playerDMG, fontIncrement);
        else
            CoreUIManager.Instance.SpawnText(dmg.ToString(), target, enemyDMG, fontIncrement);
    }

    public void ReceiveIncidentalDamage(int damage, MagicElemancy element)
    {
        DamageInfo damageInfo = ResolveDamageUnitAGivesUnitB(this.Properties, this.Properties, damage);
        int dmg = (int)damageInfo.TotalDamage;
        this.Properties.ChangeHealth(-dmg);
        this.SpawnDamageParticles(element);

        CoreUIManager.Instance.SpawnText(dmg.ToString(), this.gameObject, CoreUIManager.Instance.PlayerTakeDamageColor, 1.6f);
    }

    public void Revive()
    {
        if (State != ActorState.Dead)
            GameManager.Instance.LogError("Trying to revive actor " + this.name + " while alive.");

        EnterIdleState();
        TargetObject = null;
        SoftTargetObject = null;
        RestoreHP(30, true);
    }

    public void RestoreHP(int percentage, bool showDamageNumber)
    {
        float toRestore = this.Properties.MaxHealth * (float)percentage / 100f;
        this.Properties.ChangeHealth((int)toRestore);

        if (showDamageNumber)
        {
            string show = ((int)toRestore).ToString();
            CoreUIManager.Instance.SpawnText(show, this.gameObject, CoreUIManager.Instance.RestoreHealthColor, 1.8f);
        }
    }

    public void PullAggro(GameObject puller)
    {
        if (TargetObject != null)
            previousTarget = TargetObject;
        SetTarget(puller);
    }

    public void EnterDeathState()
    {
        if (State == ActorState.Dead)
        {
            GameManager.Instance.Log("Going into Death state while already in that state for unit " + this.name);
            return;
        }

        State = ActorState.Dead;
        OnDestroyCallBack.Invoke();
        GameManager.Instance.Log("Actor " + this.name + " has died");
    }

    public void EnterIdleState()
    {
        if (State == ActorState.Idle)
        {
            GameManager.Instance.Log("Going into Idle state while already in that state for unit " + this.name);
            return;
        }
        State = ActorState.Idle;
        EnterIdleStateCallBack.Invoke();
        GetComponent<BaseAI>().ResetOnEnterIdleState(); 
        GameManager.Instance.Log("Actor " + this.name + " enters Idle state. Engaged enemies: " + EngagedEnemies.Count);
    }

    public void EnterEngagedState()
    {
        if (State == ActorState.Engaged)
        {
            GameManager.Instance.Log("Going into Engaged state while already in that state for unit " + this.name);
            return;
        }

        State = ActorState.Engaged;
        EnterEngageStateCallBack.Invoke();
        GameManager.Instance.Log("Actor " + this.name + " enters Engaged state. Engaged enemies: " + EngagedEnemies.Count);
    }

    public void DisengageAllEngagedEnemies(bool notifyEnemy = false)
    {
        if(notifyEnemy)
        {
            foreach (var unit in EngagedEnemies)
            {
                unit.GetComponent<RPGActor>().DisengageAllEngagedEnemies();
            }
        }

        EngagedEnemies.Clear();
        EnterIdleState();

        SoftTargetObject = null;
        TargetObject = null;
    }

    public void Disengage(GameObject gO)
    {
        if (EngagedEnemies.Count == 0)
            return;

        bool removed = EngagedEnemies.Remove(gO);

        if (!removed)
            GameManager.Instance.LogError("Couldn't find gameObject " + gO.name + "in actor " + this.name + "'s list of engaged enemies");
    }

    public void SetTarget(GameObject target)
    {
        this.TargetObject = target;
    }

    public void SetSoftTarget(GameObject target)
    {
        this.SoftTargetObject = target;
    }

    public void UnsetSoftTarget()
    {
        this.SoftTargetObject = null;
    }

    public void TargetSoftTarget()
    {
        SetTarget(SoftTargetObject);
    }

    public void EngageTarget(bool notifyEnemy = false) 
    {
        Assert.IsNotNull(TargetObject, "Trying to Engage Target that isn't there, set a target with SetTarget()");

        //Don't engage when we're already engaged with this enemy
        if (EngagedEnemies.Contains(TargetObject))
            return;

        //Notify the Game Manager the first time around
        if(GameManager.Instance.CurrentState == GameManager.Instance.StateIdle)
            GameManager.Instance.EnterBattleState();

        //State = ActorState.Engaged;
        EnterEngagedState();

        //If we engage the player we engage the whole party!
        if(this.TargetObject.tag == "Player")
        {
            foreach (var member in GameManager.Instance.CurrentPartyMembers)
            {
                if (!EngagedEnemies.Contains(member))
                    EngagedEnemies.Add(member);
            }
        }
        else
        {
            if (EngagedEnemies.Contains(this.TargetObject))
                return;

            //Add to engaged enemy list if target is new
            EngagedEnemies.Add(this.TargetObject);
        }
    }

    private void Reset()
    {
        if(EnterIdleStateCallBack == null)
            EnterIdleStateCallBack = new UnityEvent();

        if (OnDestroyCallBack == null)
            OnDestroyCallBack = new UnityEvent();

        if (EnterEngageStateCallBack == null)
            EnterEngageStateCallBack = new UnityEvent();
    }

    public List<GameObject> GetNearestEnemiesForPlayer(float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, radius);
        List<GameObject> results = new List<GameObject>();

        if(hitColliders.Length > 0)
        {
            foreach (var col in hitColliders)
            {
                if(this.tag == "Player" && col.tag == "Enemy")
                { 
                    results.Add(col.gameObject);
                }
            }
        }

        return results;
    }

    public List<GameObject> GetNearestLeader(float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, radius);
        List<GameObject> results = new List<GameObject>();

        if (hitColliders.Length > 0)
        {
            foreach (var col in hitColliders)
            {
                if (this.tag == "Enemy" && col.tag == "Player")
                {
                    if(col.gameObject.GetComponent<RPGActor>().IsLeader)
                        results.Add(col.gameObject);
                }
            }
        }

        return results;
    }

    public void AddExperience(GameObject defeatedEnemyActor)
    {
        if(defeatedEnemyActor != null)
        {
            BattleResults result = GameManager.Instance.CalculateBattleResults(defeatedEnemyActor.GetComponent<RPGActor>());
            bool isLevelUp = false; 
            foreach (var unit in GameManager.Instance.CurrentPartyMembers)
            {
                var actor = unit.GetComponent<RPGActor>();
                var prop = unit.GetComponent<RPGActor>().Properties;
                if (prop.CurrentHealth > 0)
                {
                    bool levelUp = prop.IncreaseExperience(result.Experience);

                    if (levelUp)
                        isLevelUp = true;
                }
            }

            GameManager.Instance.Gold += result.Gold;

            CoreUIManager.Instance.SpawnText(result.Experience + " exp", defeatedEnemyActor);
            CoreUIManager.Instance.SpawnLabel(result.Gold + "g", this.gameObject);

            if (isLevelUp)
                CoreUIManager.Instance.ShowBigText("Level up!");
        }
        //CoreUIManager.Instance.SpawnDamageText("1 LP", this.gameObject);
    }

    public DamageInfo ResolveDamageUnitAGivesUnitB(RPGProperties Instigator, RPGProperties receiver, int skillPotency)
    {
        int weaponAttack = Instigator.WeaponAttack;

        if (isPlayer)
            weaponAttack = GetComponent<PlayerEquipment>().CurrentWeapon.DamageOutput;

        float baseDamage = weaponAttack - receiver.Defense;

        if (baseDamage <= 1.0)
            baseDamage = 1.0f;

        //multiplier
        float lowerLimit = Instigator.Strength + skillPotency * 4;
        float upperLimit = Instigator.Strength + skillPotency * 4 + (Instigator.Strength + Instigator.Level) / 4;
        float multiplier = UnityEngine.Random.Range(lowerLimit, upperLimit);

        //level
        float levelTakenInto = Mathf.Pow(Instigator.Level, 0.5f);
        float levelMultiplier = 1 + levelTakenInto;

        int lvDifference = receiver.Level - Instigator.Level;

        if (lvDifference <= 1)
            lvDifference = 1;
        
        //luck i.e critical hit (1-1.x multiplier)
        float critMultiplier = Random.Range(Instigator.Skill / 10,  0.3f + (Instigator.Skill / 8));

        if (critMultiplier < 1f)
            critMultiplier = 1f;
        
        //general scaler
        float generalScaler = 1.0f;

        if (receiver.IsBreak)
            generalScaler = 1.15f;
        if (receiver.IsTopple)
            generalScaler = 1.3f;

        float totalDamage = baseDamage * multiplier * levelMultiplier * critMultiplier * generalScaler * 0.1f;
        totalDamage /= lvDifference;

        DamageInfo dmgInfo;
        dmgInfo.TotalDamage = totalDamage;
        dmgInfo.CritMultiplier = critMultiplier;
        dmgInfo.CriticalDamage = 0;

        return dmgInfo;
    }

    public struct DamageInfo
    {
        public float TotalDamage;
        public float CriticalDamage;
        public float CritMultiplier;
    }

    public bool IsBehindTarget()
    {
        Vector3 toTarget = (TargetObject.transform.position - transform.position).normalized;

        float dt = Vector3.Dot(toTarget, TargetObject.transform.forward);
        //GameManager.Instance.Log(dt);

        if (dt < 0)
            return false; //in front
        else if (dt > 0)
            return true;

        return false;
    }

    #region Commands

    private void ExecuteCommand(Command command)
    {
        if (command.ActionVariable != null)
            command.ActionVariable.Invoke();
    }

    public void ResetCommands()
    {
        foreach (var command in PartyMemberCommands)
            command.ResetCommand();
    }

    public Command GetCommandAtSlotIndex(int i)
    {
        foreach (var command in PartyMemberCommands)
        {
            if (command.Slot == i)
                return command;
        }

        return null;

        /*
        if (PartyMemberCommands.Count > i)
            return PartyMemberCommands[i];
        return null;
        */
    }
   
    #endregion
}
