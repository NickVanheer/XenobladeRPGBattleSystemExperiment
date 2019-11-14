using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReviveAlly : MonoBehaviour {

    public float DiscoverRange = 10;
    GameObject ally;
    bool isUIShown = false;
	// Use this for initialization
	void Start () {
        CoreUIManager.Instance.HideRevivePrompt();

    }
	
	// Update is called once per frame
	void Update () {

        if (this.gameObject.GetComponent<RPGActor>().State != ActorState.Engaged || this.gameObject.GetComponent<RPGActor>().State == ActorState.Dead)
            return;

        if (isUIShown)
        {
            CoreUIManager.Instance.ShowRevivePrompt();

            if(Input.GetKeyDown(KeyCode.R) && ChainBarDisplayController.Instance.HasSegmentReady())
            {
                ChainBarDisplayController.Instance.UseChainSegment();
                ally.GetComponent<RPGActor>().Revive();
                CoreUIManager.Instance.HideRevivePrompt();
            }

        }
        else
        {
            CoreUIManager.Instance.HideRevivePrompt();
        }

        //do every second or so to save on fps(?)
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, DiscoverRange);
        isUIShown = false;

        if (hitColliders.Length > 0)
        {
            foreach (var col in hitColliders)
            {
                if (col.tag == "Player" && col.gameObject.GetComponent<RPGActor>().Properties.CurrentHealth <= 0)
                {
                    ally = col.gameObject;
                    isUIShown = true;
                    break;
                }
            }
        }
    }
}
