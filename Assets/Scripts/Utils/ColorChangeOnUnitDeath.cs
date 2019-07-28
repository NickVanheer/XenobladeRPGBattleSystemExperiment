using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeOnUnitDeath : MonoBehaviour {

    public Color DeadColor;
    private Color aliveColor;
    Material curentMaterial;

    private void Start()
    {
        curentMaterial = GetComponent<MeshRenderer>().material;
        aliveColor = curentMaterial.color;
    }

    void Update () {

        if(transform.parent.GetComponent<RPGActor>().State == ActorState.Dead)
            curentMaterial.color = DeadColor;
        else
            curentMaterial.color = aliveColor;
    }
}
