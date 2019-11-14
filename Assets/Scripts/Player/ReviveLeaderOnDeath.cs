using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveLeaderOnDeath : MonoBehaviour {

	void Update () {

        RPGActor current = GetComponent<RPGActor>();
        if (current.State == ActorState.Engaged && GameManager.Instance.GetLeaderActor().State == ActorState.Dead && ChainBarDisplayController.Instance.HasSegmentReady())
        {
            ChainBarDisplayController.Instance.UseChainSegment();
            GameManager.Instance.GetLeaderActor().Revive();
            CoreUIManager.Instance.SpawnLabel("('ω')ノ", GameManager.Instance.GetPartyLeader());
        }
    }
}
