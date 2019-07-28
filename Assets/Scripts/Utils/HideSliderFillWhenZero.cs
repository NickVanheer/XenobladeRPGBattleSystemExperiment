using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideSliderFillWhenZero : MonoBehaviour {

    public Image SliderFill;
    public Slider SliderToUse;
    void Start()
    {
        if (SliderToUse == null)
            SliderToUse = GetComponent<Slider>();

        if (SliderFill == null)
            SliderFill = transform.FindDeepChild("Fill").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (SliderToUse.value <= 1)
            SliderFill.enabled = false;
        else
            SliderFill.enabled = true;

    }
}
