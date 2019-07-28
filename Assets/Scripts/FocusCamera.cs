using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusCamera : MonoBehaviour {

    public GameObject TargetObject;
    public Transform oldPosition;
    public bool IsFocussing = false;
    public Vector3 Offset;
    public float Duration = 3f;
    public float LerpFactor = 0.1f;

    private float timer;
	
	void Update () {
        if (IsFocussing)
        {
            timer -= Time.deltaTime;
            transform.position = Vector3.Lerp(GetComponent<Camera>().transform.position, TargetObject.transform.position + Offset, LerpFactor);

            if(timer <= 0)
            {
                IsFocussing = false;
                transform.position = oldPosition.position;
                GetComponent<PlayerCameraFollow>().enabled = true;
            }
        }
    }

    public void Focus(GameObject gObj)
    {
        TargetObject = gObj;
        oldPosition = transform;
        GetComponent<PlayerCameraFollow>().enabled = false;
        IsFocussing = true;
        timer = Duration;


    }
}
