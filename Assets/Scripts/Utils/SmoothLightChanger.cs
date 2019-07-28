using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SmoothLightChanger : MonoBehaviour {

    public Light LightToChange;

    public void Start()
    {
        //StartCoroutine(fadeInAndOut(LightToChange, true, 3, 0.3f, 2.4f));
    }

    public void ChangeLight(bool fadeIn, float duration, float minLuminosity, float maxLuminosity, UnityAction callback = null)
    {
        StartCoroutine(fadeInAndOut(LightToChange, fadeIn, duration, minLuminosity, maxLuminosity, callback));

    }

    IEnumerator fadeInAndOut(Light lightToFade, bool fadeIn, float duration, float minLuminosity, float maxLuminosity, UnityAction callback = null)
    {
        /*
        float minLuminosity = 0.3f; // min intensity
        float maxLuminosity = 2.4f; // max intensity
        */

        float counter = 0f;

        //Set Values depending on if fadeIn or fadeOut
        float a, b;

        if (fadeIn)
        {
            a = minLuminosity;
            b = maxLuminosity;
        }
        else
        {
            a = maxLuminosity;
            b = minLuminosity;
        }

        float currentIntensity = lightToFade.intensity;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            lightToFade.intensity = Mathf.Lerp(a, b, counter / duration);
            yield return null;
        }

        if (callback != null)
            callback();
    }
}
