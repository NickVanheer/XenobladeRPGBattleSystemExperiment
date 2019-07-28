using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMoveRandomizer : MonoBehaviour {

    // Use this for initialization
    public float Speed;
    public int Direction;
    public float MoveDuration = 20;

    private float timer; 

	void Start () {
        Speed = Random.Range(1f, 3);
        Direction = Random.Range(0, 2) * 2 - 1;
    }
	
	void Update () {
        this.transform.Translate(this.transform.forward * Direction * Speed * Time.deltaTime);
        timer += Time.deltaTime;

        if(timer >= MoveDuration)
        {
            timer = 0;
            Direction *= -1;
        }
	}
}
