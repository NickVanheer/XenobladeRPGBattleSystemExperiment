using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomWhenInRange : MonoBehaviour {

    public GameObject Player;
    public Camera MainCamera;

    public float Range;
    public int ZoomLevelInRange = 3;
    public int ZoomLevelOutRange = 4;

    public float DistanceToPlayer;

    private bool isEnabled = false;
    private bool zoomLevelSet = false;

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

        DistanceToPlayer = Mathf.Abs(Vector3.Distance(Player.transform.position, this.transform.position));

        //Ensures it will only start when in range
        if(DistanceToPlayer < Range)
            isEnabled = true;

        if(isEnabled)
            OnEnabledUpdate();
    }

    private void OnEnabledUpdate()
    {
        if (zoomLevelSet && DistanceToPlayer > Range)
        {
            MainCamera.GetComponent<PlayerCameraFollow>().SetZoomLevel(ZoomLevelOutRange);
            zoomLevelSet = false;
            isEnabled = false;
        }

        if (!zoomLevelSet && DistanceToPlayer < Range)
        {
            MainCamera.GetComponent<PlayerCameraFollow>().SetZoomLevel(ZoomLevelInRange);
            zoomLevelSet = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
