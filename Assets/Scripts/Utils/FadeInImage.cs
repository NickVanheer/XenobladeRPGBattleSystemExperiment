using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInImage : MonoBehaviour {

    public Image ImageToFade;

    void Start()
    {
        if (ImageToFade == null)
            ImageToFade = GetComponent<Image>();

        if (!ImageToFade.enabled)
            ImageToFade.enabled = true;
    }

    void Update () {
        ImageToFade.CrossFadeAlpha(0, 2f, false);
    }
}
