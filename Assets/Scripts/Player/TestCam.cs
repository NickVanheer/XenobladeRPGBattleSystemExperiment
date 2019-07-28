using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class TestCam : MonoBehaviour {

    public GameObject Target;
    public Vector3 Offset;
    public float LerpFactor = 0.25f;

    Vector3 targetPos;
    private float interpVelocity;
    private Vector3 originalOffset;

    public float ScrollSpeed = 1;
    public bool IsLookAt = false;
    // Use this for initialization
    void Start()
    {
        targetPos = transform.position;
        originalOffset = Offset;

       Assert.IsNotNull<GameObject>(Target, "TestCam > No tracking target assigned.");
    }

    void FixedUpdate()
    {
        if (Target)
        {
            Vector3 targetDirection = Target.transform.position - transform.position;
            interpVelocity = targetDirection.magnitude * 5f;

            targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPos + Offset, LerpFactor);

            if(IsLookAt)
                transform.LookAt(targetPos, Vector3.up);

            //zoom
            /*
            float f = Input.mouseScrollDelta.y * ScrollSpeed;

            if (f > 0.01f || f < -0.01f)
            {
                Offset.y -= f;
                Offset.y = Mathf.Clamp(Offset.y, 1.5f, 9f);
            }
            */
        }
    }

    public void RaiseCameraHeight(float newValue)
    {
        this.Offset.x = 0;
        this.Offset.y = newValue;
        this.Offset.z = -newValue;
    }

    public void IncrementOffset(Vector3 offset)
    {
        this.Offset.x += offset.x;
        this.Offset.y += offset.y;
        this.Offset.z += offset.z;
    }

    public void SetZoomOffset(float value)
    {
        Offset.y = value;
        //Offset.y = Mathf.Clamp(Offset.y, 1.5f, 9f);
    }

    public void ResetZoomOffset()
    {
        Offset = originalOffset;
    }
}
