using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPickup : MonoBehaviour {

    public int GoldToAdd = 200;

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log(collision.gameObject.tag);

        if (collision.gameObject.tag == "Player")
        {
            GameManager.Instance.Gold += GoldToAdd;
            CoreUIManager.Instance.SpawnLabel("+" + GoldToAdd + " " + LocalizationManager.Instance.GetLocalizedValue("Gold"), GameManager.Instance.GetPartyLeader());
            Destroy(this.gameObject);
        }
    }

}
