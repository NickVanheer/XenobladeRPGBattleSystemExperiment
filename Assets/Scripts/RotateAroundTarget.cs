using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundTarget : MonoBehaviour {

    public float TurnSpeed = 4.0f;
    public Transform target;

    void Update()
    {
        transform.RotateAround(target.position, new Vector3(0.0f, 1.0f, 0.0f), Time.deltaTime * TurnSpeed);
    }
}
