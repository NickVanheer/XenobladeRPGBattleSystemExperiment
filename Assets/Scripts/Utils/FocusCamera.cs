using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum FocusMode
{
    Slide, Static
}

public class FocusCamera : MonoBehaviour {

    public GameObject TargetObject;
    public Transform oldPosition;
    public bool IsFocussing = false;
    public Vector3 Offset;
    public float Duration = 3f;
    public float LerpFactor = 0.1f;

    private float timer;
	
    public void SlideCameraFocus(GameObject gObj, float duration, UnityAction callBack = null)
    {
        StartCoroutine(focusCameraCoRoutine(gObj, duration, callBack));
    }

    public void StaticCameraFocus(GameObject gObj, float duration, UnityAction callBack = null)
    {
        StartCoroutine(focusCameraCoRoutineStatic(gObj, duration, callBack));
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


    IEnumerator focusCameraCoRoutineStatic(GameObject gObj, float duration, UnityAction callBack = null)
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
            transform.position = TargetObject.transform.position + Offset;
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
