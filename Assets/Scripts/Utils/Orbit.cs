using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public float TurnSpeed = 2.0f;
    public float StartOffsetY = 16f;
    public float StartOffsetZ = 16f;
    public Vector3 WorldOffsets;
    public Vector3 LocalOffsets;
    public Transform Player;

    public float ScrollSpeed = 1;

    private Vector3 offset;
    private Vector3 startPos;

    bool isMouse = false;

    void Start()
    {
        startPos.x = Player.position.x;
        startPos.y = Player.position.y;
        startPos.z = Player.position.z;

        offset = new Vector3(startPos.x, startPos.y + StartOffsetY, startPos.z + StartOffsetZ);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1)) { isMouse = true; }
        if (Input.GetMouseButtonUp(1)) { isMouse = false; }
    }

    void LateUpdate()
    {
        if (isMouse)
        {
            offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * TurnSpeed, Vector3.up) * offset;
            transform.position = Player.position + offset;
      
            transform.LookAt(Player.position);
        }
        else
        {
            transform.position = Player.position + offset; 
            transform.LookAt(Player.position);
        }

        transform.Translate(WorldOffsets, Space.World);
        transform.Translate(LocalOffsets, Space.Self);
    }
}
