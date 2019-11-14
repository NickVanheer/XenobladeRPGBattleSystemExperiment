using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPartyLeader : MonoBehaviour {

    RPGActor actor;
    public float Distance;

    private CharacterController controller;
    private Vector3 velocity;
    public float Gravity = -9.81f;
    public float MoveSpeed = 10.0f;
    public Vector3 Drag;

    public bool IsMoving { get; private set; }
    
    void Start () {
        actor = GetComponent<RPGActor>();
        controller = GetComponent<CharacterController>();
    }
	
	void Update () {

        if (GameManager.Instance.IsPausedForUI)
            return;

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = 0f;

        if (actor.State == ActorState.Idle)
        {
            var leader = GameManager.Instance.GetPartyLeader();

            if (leader == null)
                return;

            float distance = Vector3.Distance(this.transform.position, leader.transform.position);

            if (Mathf.Abs(distance) > Distance)
                IsMoving = true;

            if (IsMoving)
            {
                if (Mathf.Abs(distance) >= 18)
                {
                    //RotateTowardsTarget
                    var targetRotation = Quaternion.LookRotation(leader.transform.position - transform.position);
                    Quaternion q = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, q.eulerAngles.y, 0);

                    controller.Move(transform.forward * Time.deltaTime * MoveSpeed);
                }
                else
                {
                    IsMoving = false;
                }
            }

            //Gravity 
            velocity.y += Gravity * Time.deltaTime;

            velocity.x /= 1 + Drag.x * Time.deltaTime;
            velocity.y /= 1 + Drag.y * Time.deltaTime;
            velocity.z /= 1 + Drag.z * Time.deltaTime;

            //Apply gravity
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
