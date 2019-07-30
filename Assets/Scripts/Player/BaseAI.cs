using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AIAnimation { Idle, Moving, Attacking}; 
public abstract class BaseAI : MonoBehaviour {

    public float AttackTimer = 2;
    public float MoveSpeed = 10;
    public float StartAttackTimer = 0;
    public bool CanFollowTarget = false;

    private float currentAttackTimer;
    protected RPGActor actor;

    public float ClosestEngageDistance = 10;
    public float FarthestEngageDistance = 30;

    private GameObject playerOverheadHealthbar;
    private Vector3 moveOffset = Vector3.zero;

    void Start () {
        actor = GetComponent<RPGActor>();
        CalculateAutoAttackDelayBasedOnSpeed();
        currentAttackTimer = StartAttackTimer;
    }
	
	public virtual void Update () {

        moveOffset = Vector3.zero;
        if(actor.State == ActorState.Engaged && actor.TargetObject != null)
        {
            //Only create the overhead healthbar when we're a player.
            if (playerOverheadHealthbar == null && gameObject.tag == "Player")
                playerOverheadHealthbar = CoreUIManager.Instance.CreateOverheadHealthBar(this.gameObject);

            if(playerOverheadHealthbar != null)
                playerOverheadHealthbar.gameObject.SetActive(true);

            //check if target died and if it has, assign the next target, (if there is none, disengage)
            if (actor.TargetObject.GetComponent<RPGActor>().State == ActorState.Dead)
            {
                foreach (var target in actor.EngagedEnemies)
                {
                    if (target.GetComponent<RPGActor>().State == ActorState.Engaged)
                    {
                        this.GetComponent<RPGActor>().SetTarget(target);
                        break;
                    }
                }
            }

            //Don't do anything when we're Toppled.
            if (actor.Properties.IsTopple)
                return;

            //Rotate leader towards target when we're not moving ourselves
            if(actor.IsLeader  && !GetComponent<PlayerMove>().IsMoving)
                rotateTowardsTarget();
            else if(!GetComponent<RPGActor>().IsLeader) //rotate other party members and enemies to target too
                rotateTowardsTarget();

            UpdateLoop();

            //
            var heading = actor.TargetObject.transform.position - this.transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance; // This is now the normalized direction.

            if (Mathf.Abs(distance) <= FarthestEngageDistance && Mathf.Abs(distance) >= ClosestEngageDistance)
            {
                currentAttackTimer -= Time.deltaTime;
                moveOffset = Vector3.zero;

                if (currentAttackTimer <= 0)
                {
                    DefaultAttack(); //overrides in derived
                    currentAttackTimer = AttackTimer;

                    //Add to chain gauge
                    ChainBarDisplayController.Instance.AddToChainBar(2);
                }
            }
            else
            {
                //We haven't made contact yet, move closer to target. Or we're marked to always follow the target
                //Move back
                if (Mathf.Abs(distance) < ClosestEngageDistance)
                    direction = direction * -1;

                direction.y = 0;
                moveOffset = direction;
            }
        }

        if(actor.State != ActorState.Engaged && playerOverheadHealthbar != null)
            playerOverheadHealthbar.gameObject.SetActive(false);

        //Restore health at same interval as auto-attack tick when we're not in battle (so when we're idle
        if (actor.State != ActorState.Idle && actor.Properties.CurrentHealth != actor.Properties.MaxHealth)
        {
            //Gradually restore our health
            currentAttackTimer -= Time.deltaTime;

            if (currentAttackTimer <= 0)
            {
                actor.RestoreHP(25, true);
                currentAttackTimer = AttackTimer;
            }
        }
	}

    public void LateUpdate()
    {
        if (CanFollowTarget)
            moveCloserToTarget(moveOffset);
    }

    private void moveCloserToTarget(Vector3 directionVector)
    {
        bool isMoving = false;
        PlayerMove moveControl = GetComponent<PlayerMove>();

        if (moveControl != null)
            isMoving = moveControl.IsMoving;

        if (!isMoving)
        {
            CharacterController controller = GetComponent<CharacterController>();

            if(controller != null)
                controller.Move(directionVector * MoveSpeed * Time.deltaTime);
            else
                transform.Translate(directionVector * MoveSpeed * Time.deltaTime, Space.World);
        }
    }

    private void rotateTowardsTarget()
    {
        var targetRotation = Quaternion.LookRotation(actor.TargetObject.transform.position - transform.position);
        Quaternion q = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, q.eulerAngles.y, 0);
    }

    public void CalculateAutoAttackDelayBasedOnSpeed()
    {
        //Speed: value between 1 - 25, time becomes smaller (between 0.1 - 5.5 seconds) when the speed is a higher value
        int speed = actor.Properties.Speed;
        AttackTimer = Mathf.Log((50 / speed), 2) + 1;
    }

    //Fired when the unit goes idle, i.e: clear and reset all relevant data so the enemy is like new again when the next battle starts
    public abstract void ResetOnEnterIdleState();

    //Update loop that can be overrided in derived classes and which will only be called when this unit is engaged and has a target
    public abstract void UpdateLoop();
    
    public virtual void DefaultAttack()
    {
        int dmg = Random.Range(1, 3);
        actor.DoDamageAttackOnCurrentTarget(dmg);
    }

    public Vector3 GetFloorPosition()
    {
        GameObject gO = GameManager.Instance.GetFloorInUnit(this.transform.gameObject);
        return gO.transform.position;
    }

}
