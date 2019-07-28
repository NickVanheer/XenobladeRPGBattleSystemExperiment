using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour {

    public RPGActor ActorToShow;
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
        if(ActorToShow != null)
            SetValues();
	}

    private void SetValues()
    {
        GetComponent<Slider>().maxValue = ActorToShow.Properties.MaxHealth;
        GetComponent<Slider>().minValue = 0;
        GetComponent<Slider>().value = ActorToShow.Properties.CurrentHealth;
    }
}
