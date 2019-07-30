using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundAxis : MonoBehaviour {

    public float Speed = 50f;
    public Vector3 Rotation;

	void Update () {
        transform.Rotate(Rotation.x * Time.deltaTime * Speed, Rotation.y * Time.deltaTime * Speed, Rotation.z * Time.deltaTime * Speed);
	}
}
