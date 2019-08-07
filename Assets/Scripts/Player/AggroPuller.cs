using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroPuller : MonoBehaviour {

    public float AggroTimer = 12;
    public float AggroTimerStart = 3;
    float currentTimer = 0;

    private void Start()
    {
        currentTimer = AggroTimerStart;
    }

    void Update()
    {
        //Party members should never be deleted
        RPGActor leader = GameManager.Instance.CurrentPartyMembers[0].GetComponent<RPGActor>();

        if (leader != null && leader.State == ActorState.Engaged)
        {

            //Check if the current enemy isn't targetting us already. the leader might be engaged, but its target might not have a TargetObject (i.e: the leader) yet.
            /* if (leader.Target == null)
                return; */

            if (leader.Target.TargetObject == null)
                return;

            if (leader.Target.TargetObject == this.gameObject)
                return;

            currentTimer -= Time.deltaTime;

            if (currentTimer <= 0)
            {
                CoreUIManager.Instance.SpawnLabel(LocalizationManager.Instance.GetLocalizedValue("Aggro") + "↑", this.gameObject);
                GetComponent<RPGActor>().Target.PullAggro(this.gameObject);
                currentTimer = AggroTimer;
            }
        }
    }
}
