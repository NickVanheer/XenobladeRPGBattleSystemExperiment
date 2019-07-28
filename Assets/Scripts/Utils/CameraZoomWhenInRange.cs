using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomWhenInRange : MonoBehaviour {

    public GameObject Player;
    public float Range;
    public Camera MainCamera;

    public bool IsActive = false;

    public int ZoomLevelInRange = 3;
    public int ZoomLevelOutRange = 4;
    private void Start()
    {
        if (MainCamera == null)
            MainCamera = Camera.main;

        if (Player == null)
            Player = GameManager.Instance.GetPartyLeader();
    }

    void Update () {

        if (Player == null)
            return;

        float dist = Vector3.Distance(Player.transform.position, this.transform.position);
        dist = Mathf.Abs(dist);

        if (dist <= Range)
        {
            MainCamera.GetComponent<PlayerCameraFollow>().SetZoomLevel(ZoomLevelInRange);
            IsActive = true;
        }

        if(IsActive && dist > Range)
        {
            IsActive = false;
            MainCamera.GetComponent<PlayerCameraFollow>().SetZoomLevel(ZoomLevelOutRange);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
