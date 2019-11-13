using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FocusCamera : MonoBehaviour {

    public GameObject TargetObject;
    public Transform oldPosition;
    public bool IsFocussing = false;
    public Vector3 Offset;
    public float Duration = 3f;
    public float LerpFactor = 0.1f;

    private float timer;
	
	void Update () {

        /* Implemented using coroutines
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
        */
    }

    public void Focus(GameObject gObj, float duration, UnityAction callBack = null)
    {
        StartCoroutine(focusCameraCoRoutine(gObj, duration, callBack));
        /*
        TargetObject = gObj;
        oldPosition = transform;
        GetComponent<PlayerCameraFollow>().enabled = false;
        IsFocussing = true;
        timer = duration;
        */
    }

    IEnumerator focusCameraCoRoutine(GameObject gObj, float duration, UnityAction callBack = null)
    {
        TargetObject = gObj;
        oldPosition = transform;
        GetComponent<PlayerCameraFollow>().enabled = false;
        IsFocussing = true;
        timer = duration;

        //executed over frames
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            transform.position = Vector3.Lerp(GetComponent<Camera>().transform.position, TargetObject.transform.position + Offset, LerpFactor);
            yield return null;
        }

        //end
        transform.position = oldPosition.position;
        GetComponent<PlayerCameraFollow>().enabled = true;
        IsFocussing = false;

        if (callBack != null)
            callBack();
    }
}
