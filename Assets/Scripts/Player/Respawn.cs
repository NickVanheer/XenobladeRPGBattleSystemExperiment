using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour {

    // Use this for initialization
    public GameObject RespawnPoint;
	void Start () {
        Debug.Assert(RespawnPoint != null);
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.Instance.GetPartyLeader().transform.position.y <= -80)
            RespawnAllAtSpawnPoint(RespawnPoint);
	}

    public void RespawnAllAtSpawnPoint(GameObject targetSpawnPoint)
    {
        GameManager.Instance.Log("Players respawning");
        Vector3 offset = Vector3.zero;
        foreach (var pMember in GameManager.Instance.CurrentPartyMembers)
        {
            pMember.GetComponent<RPGActor>().DisengageAllEngagedEnemies(true);
            pMember.transform.position = targetSpawnPoint.transform.position + offset;
            offset += new Vector3(5, 0, 5);
        }
    }
}
