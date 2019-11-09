using UnityEngine;
using System.Collections;

public class RotateToCamera : MonoBehaviour {

    public float X;
    public float Y;
    public float Z;

    private void Start()
    {
    }
    void Update () {
        //transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position, Vector3.up);
        this.transform.forward = Camera.main.transform.forward;
        //transform.LookAt(Camera.main.transform.position, Vector3.up);
        //transform.rotation = Quaternion.LookRotation(Vector3.zero, Vector3.up);
    }
}
