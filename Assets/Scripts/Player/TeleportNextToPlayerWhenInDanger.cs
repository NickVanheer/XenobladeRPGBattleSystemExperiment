using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportNextToPlayerWhenInDanger : MonoBehaviour {

    public float AwayDistance = 80;

	void Update () {
        bool isTeleport = false;
        Transform leader = GameManager.Instance.GetPartyLeader().transform;

        if (Vector3.Distance(leader.position, this.transform.position) > AwayDistance)
            isTeleport = true;

        if (Mathf.Abs(transform.position.y) > 30)
            isTeleport = true;

        if (isTeleport)
            this.transform.position = leader.transform.position + new Vector3(3, 0, 3);
	}
}
