using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public class PlayerCameraFollow : MonoBehaviour {

    public GameObject Player;
    public Transform LookAt;
    public float ScrollSpeed = 1;
    public float LerpFactor = 0.05f;
    public bool IsLookAt = false;

    public int zoomLevel = 3;
    public Vector3 GlobalCameraOffset;
    public Vector3 OffsetOnZoom = new Vector3(12, -19, 16);

    private Vector3 cameraVector;
    private Vector3 transitionVector;
    private bool isTransition = false;
    public Vector3 zoomOffset;
    //private Vector3 cameraOffset;

    void Start () {
        cameraVector = transform.position - Player.transform.position;

        if (LookAt == null && Player != null)
            LookAt = Player.transform;

        if (Player == null && LookAt != null)
            Player = LookAt.gameObject;

        if (LookAt == null && Player == null)
            Assert.IsFalse(true, "Either Player or LookAt should have a value");

        SetTransition(new Vector3(60, 60, 160));
       // SetZoomLevel(StartZoomLevel);
        //SetOffset(FarOffset);
    }

    void Update()
    {
        Vector3 target = Player.transform.position;
        //cameraOffset = Vector3.Lerp(cameraOffset, ZoomOffset, OffsetSpeed);

        //When we're in battle, focus on the point inbetween the player and the enemy for a better viewing angle of the action
        if(Player.GetComponent<RPGActor>().State == ActorState.Engaged && Player.GetComponent<RPGActor>().TargetObject != null)
            target = (Player.transform.position + Player.GetComponent<RPGActor>().TargetObject.transform.position) / 2;

        transform.position = Vector3.Lerp(transform.position, target + cameraVector + zoomOffset + transitionVector + GlobalCameraOffset, LerpFactor);

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

        zoomOffset = zoomOffset + new Vector3(-OffsetOnZoom.x, -OffsetOnZoom.y, -OffsetOnZoom.z) * 0.5f;
        zoomLevel--;
    }

    public void ZoomIn()
    {
        if (zoomLevel + 1 == 8)
            return;

        zoomOffset = zoomOffset + new Vector3(OffsetOnZoom.x, OffsetOnZoom.y, OffsetOnZoom.z) * 0.5f;
        zoomLevel++;
    }

    public void SetOffset(Vector3 pos)
    {
        zoomOffset = pos;
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
