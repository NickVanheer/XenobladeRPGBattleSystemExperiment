using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChainBarDisplayController : MonoBehaviour {

    static ChainBarDisplayController instance;

    public static ChainBarDisplayController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ChainBarDisplayController();
            }
            return instance;
        }
    }

    public int ChainBarsReady = 0;
    public Slider ChainGauge1;
    public Slider ChainGauge2;
    public Slider ChainGauge3;

    public float Scaler = 0.3f;

    public void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void AddToChainBar(float value)
    {
        value = value * Scaler;

        if (ChainBarsReady == 0)
            ChainGauge1.value += value;
        else if (ChainBarsReady == 1)
            ChainGauge2.value += value;
        else if(ChainBarsReady == 2)
            ChainGauge3.value += value;

        ChainGauge1.value = Mathf.Clamp(ChainGauge1.value, 0, ChainGauge1.maxValue);
        ChainGauge2.value = Mathf.Clamp(ChainGauge2.value, 0, ChainGauge2.maxValue);
        ChainGauge3.value = Mathf.Clamp(ChainGauge3.value, 0, ChainGauge3.maxValue);

        if (ChainGauge1.value == ChainGauge1.maxValue)
            ChainBarsReady = 1;
        if (ChainGauge2.value == ChainGauge2.maxValue)
            ChainBarsReady = 2;
        if (ChainGauge3.value == ChainGauge3.maxValue)
            ChainBarsReady = 3;
    }

    public bool HasSegmentReady()
    {
        return ChainBarsReady > 0;
    }

    public void ResetValues()
    {
        ChainGauge1.value = 0;
        ChainGauge2.value = 0;
        ChainGauge3.value = 0;

        ChainBarsReady = 0;
    }

    public bool UseChainSegment()
    {
        bool isOkay = false;
        if(HasSegmentReady())
        {
            isOkay = true;

            if (ChainBarsReady == 1)
            {
                ChainGauge1.value = ChainGauge2.value;
                ChainGauge2.value = 0;
            }
            else if (ChainBarsReady == 2)
            {
                ChainGauge2.value = ChainGauge3.value;
                ChainGauge3.value = 0;
            }
            else if (ChainBarsReady == 3)
            {
                ChainGauge3.value = 0;
            }

            ChainBarsReady--;
        }

        return isOkay;
    }
}
