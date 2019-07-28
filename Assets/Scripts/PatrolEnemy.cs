using UnityEngine;
using System.Collections;

public class PatrolEnemy : MonoBehaviour {

    [Range(1f, 250.0f)]
    public float MoveSpeed = 20;

    [Range(1f, 100.0f)]
    public float Slerp = 20;

    [Range(1f, 150.0f)]
    public float HorizontalRange = 10;

    public bool IsDestroyAtEnd = false;
    public bool IsIgnoreY = false;

    public Vector3 TargetPosition;
    public Vector3 OriginalPosition;

    public float EndWaitTime = 0;
    private float endWaitTimer = 0;

    private bool isAtEnd = false;

	void FixedUpdate () {

        if(GetComponent<RPGActor>().State == ActorState.Idle)
            MoveEnemy();
	}

    private void Start()
    {
        TargetPosition = this.transform.position + (this.transform.forward * HorizontalRange);
        OriginalPosition = this.transform.position - (this.transform.forward * HorizontalRange);

        endWaitTimer = EndWaitTime;
    }

    void MoveEnemy()
    {
        BaseAI aiBase = GetComponent<BaseAI>();

        if (!isAtEnd)
        {
            Vector3 direction = TargetPosition - this.transform.position;

            var targetRotation = Quaternion.LookRotation(TargetPosition - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Slerp * Time.deltaTime);

            this.transform.Translate(direction.normalized * MoveSpeed * Time.deltaTime, Space.World);
            float dist = Vector3.Distance(this.transform.position, TargetPosition);

            if(Mathf.Abs(dist) <= 2.0f)
            {
                isAtEnd = true;
            }
        }
        else
        {
            //we wait a little...
            endWaitTimer -= Time.deltaTime;

            if (endWaitTimer <= 0)
            {
                endWaitTimer = EndWaitTime;
                Vector3 temp = OriginalPosition;
                OriginalPosition = TargetPosition;
                TargetPosition = temp;
                isAtEnd = false;
            }
        }
    }
}
