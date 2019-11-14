using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World3DUIObject : MonoBehaviour {

    public Transform Target;
    public Vector3 Offset;
    public Image SliderFill;

	void Update () {
        Vector3 PosOnScreen3D = Camera.main.WorldToScreenPoint(Target.position);
        transform.position = PosOnScreen3D + Offset;
    }

    public void SetImageFillColor(Color c)
    {
        SliderFill.color = c;
    }
}
