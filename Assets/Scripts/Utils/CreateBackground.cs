using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBackground : MonoBehaviour {

    public Transform Prefab;
    public float Radius = 15;
    public float MinScale = 0.5f;
    public float MaxScale = 0.5f;

    public float Iterations = 30;

    GameObject group;

    void Start () {

    
        group = new GameObject("Background Objects");
        for (int i = 0; i < Iterations; i++)
        {
            CreateObject();
        }
	}
	
	void Update () {
	}

    void CreateObject()
    {
        Transform t = Instantiate(Prefab);
        t.localPosition = this.transform.position + Random.insideUnitSphere * Radius;
        //t.localRotation = Random.rotation;
        t.localScale = Vector3.one * Random.Range(MinScale, MaxScale);
        t.parent = group.transform;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
