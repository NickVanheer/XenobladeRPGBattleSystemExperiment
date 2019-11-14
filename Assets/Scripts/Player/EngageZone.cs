using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngageZone : MonoBehaviour {

    public float EngageZoneDistance;
    public float EngageZoneDistanceClosest;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, EngageZoneDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, EngageZoneDistanceClosest);
    }
}
