using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionPickup : MonoBehaviour {

    public float FallSpeed = 2;
    public void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.down * FallSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameManager.Instance.RestoreAllHP(30, true);
            Destroy(this.gameObject);
        }
    }

}
