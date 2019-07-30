using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionPickup : MonoBehaviour {

    public float FallSpeed = 2;
    public void FixedUpdate()
    {
        if (!GetComponent<BoxCollider>().isTrigger)
            GetComponent<Rigidbody>().AddForce(Vector3.down * FallSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //If the potion has fallen to the floor, make it a trigger and set it to kinematic so we can pick it up well.
        GetComponent<BoxCollider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameManager.Instance.RestoreAllHP(30, true);
            Destroy(this.gameObject);
        }
    }

}
