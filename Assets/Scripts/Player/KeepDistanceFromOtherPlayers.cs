using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepDistanceFromOtherPlayers : MonoBehaviour {

    // Use this for initialization
    public float Speed = 10f;
    public float Distance = 10;
    public float DistanceWhenEngaged = 40;
    //public bool IsMoving = false;

    private Vector3 offset = Vector3.zero;

	void Update () {

        offset = Vector3.zero;

        //Can't keep distance when we're dead.
        if (this.gameObject.GetComponent<RPGActor>().State == ActorState.Dead)
            return;

        float dist = GetComponent<RPGActor>().State == ActorState.Engaged ? DistanceWhenEngaged : Distance;

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, dist);

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
                var directionVector = transform.position - closeObject.transform.position;

                var distance = directionVector.magnitude;
                var direction = directionVector / distance; // This is now the normalized direction.
                //dir.y = 0;
                //this.transform.Translate(dir.normalized * Time.deltaTime * Speed, Space.World);
                //GetComponent<CharacterController>().Move(direction * Time.deltaTime * Speed);

                offset += direction;
            }
        }
    }

    private void LateUpdate()
    {
        CharacterController c = GetComponent<CharacterController>();

        if (c != null)
            GetComponent<CharacterController>().Move(offset * Time.deltaTime * Speed);
    }
}
