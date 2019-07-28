using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class PlayerCameraFollow : MonoBehaviour {

    public GameObject Player;
    public Transform LookAt;
    public float ScrollSpeed = 1;
    public float LerpFactor = 0.05f;

    private Vector3 offset;

    public Vector3 CameraOffset;
    public bool IsLookAt = false;

    private Vector3 transitionVector;
    private bool isTransition = false;

    private Vector3 targetOffset;

    public Vector3 NearOffset = new Vector3(12,-19,16);
    public Vector3 FarOffset = new Vector3(0,-9,0);
    public float offsetSpeed = 0.25f;

    public int StartZoomLevel = 4;
    public int zoomLevel = 4;

    void Start () {
        offset = transform.position - Player.transform.position;

        if (LookAt == null && Player != null)
            LookAt = Player.transform;

        if (Player == null && LookAt != null)
            Player = LookAt.gameObject;

        if (LookAt == null && Player == null)
            Assert.IsFalse(true, "Either Player or LookAt should have a value");

        SetTransition(new Vector3(60, 60, 160));
        SetZoomLevel(StartZoomLevel);
        //SetOffset(FarOffset);
    }

    void Update()
    {
        Vector3 target = Player.transform.position;
        CameraOffset = Vector3.Lerp(CameraOffset, targetOffset, offsetSpeed);

        //When we're in battle, focus on the point inbetween the player and the enemy for a better viewing angle of the action
        if(Player.GetComponent<RPGActor>().State == ActorState.Engaged && Player.GetComponent<RPGActor>().TargetObject != null)
            target = (Player.transform.position + Player.GetComponent<RPGActor>().TargetObject.transform.position) / 2;

        transform.position = Vector3.Lerp(transform.position, target + offset + CameraOffset + transitionVector, LerpFactor);

        if (IsLookAt)
            transform.LookAt(LookAt.transform);

        if (Input.GetKeyDown(KeyCode.U))
            ZoomOut();

        if (Input.GetKeyDown(KeyCode.I))
            ZoomIn();

        if (isTransition)
        {
            transitionVector = Vector3.Lerp(transitionVector, new Vector3(0, 0, 0), 0.05f);

            if (transitionVector == Vector3.zero)
                isTransition = false;
        }
    }

    public void ZoomOut()
    {
        if (zoomLevel - 1 <= 0)
            return;

        targetOffset = targetOffset + new Vector3(-12, 19, -16) * 0.5f;
        zoomLevel--;
    }

    public void ZoomIn()
    {
        if (zoomLevel + 1 == 8)
            return;

        targetOffset = targetOffset + new Vector3(12, -19, 16) * 0.5f;
        zoomLevel++;
    }

    public void SetOffset(Vector3 pos)
    {
        targetOffset = pos;
    }

    public void SetTransition(Vector3 vec)
    {
        transitionVector = vec;
        isTransition = true;
    }

    public void SetZoomLevel(int strenght)
    {
        int diff = strenght - zoomLevel; 

        if(diff > 0)
        {
            for (int i = 0; i < diff; i++)
                ZoomIn();
        }

        if (diff < 0)
        {
            for (int i = 0; i < Mathf.Abs(diff); i++)
                ZoomOut();
        }
    }
}
