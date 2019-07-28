using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepDistanceFromOtherPlayers : MonoBehaviour {

    // Use this for initialization
    public float Speed = 10f;
    public float Distance = 10;
    //public bool IsMoving = false;

	void Update () {

        //Can't keep distance when we're dead.
        if (this.gameObject.GetComponent<RPGActor>().State == ActorState.Dead)
            return; 

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, Distance);

        if (hitColliders.Length > 0)
        {
            GameObject closeObject = null;
            foreach (var col in hitColliders)
            {
                if (this.gameObject == col.gameObject)
                    continue;

                if (/* tag == "Player" && */ col.tag == "Player")
                {
                    closeObject = col.gameObject;
                    break;
                }
            }

            if(closeObject != null)
            {
                var dir = transform.position - closeObject.transform.position;
                //dir.y = 0;
                //this.transform.Translate(dir.normalized * Time.deltaTime * Speed, Space.World);
                GetComponent<CharacterController>().Move(dir.normalized * Time.deltaTime * Speed);
            }
        }

    }
}
