using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeActorForAllUI : MonoBehaviour {

    public RPGActor ActorToDisplay;
	// Use this for initialization
	void Start () {

        List<SetUIText> list = this.transform.FindChildComponents<SetUIText>();
        foreach (var item in list)
        {
            item.ActorData = ActorToDisplay;
        }
    }
}
