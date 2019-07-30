using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World3DUIObject : MonoBehaviour {

    public Transform Target;
    public Vector3 Offset;

	void Update () {
        Vector3 PosOnScreen3D = Camera.main.WorldToScreenPoint(Target.position);
        transform.position = PosOnScreen3D + Offset;
    }
}
