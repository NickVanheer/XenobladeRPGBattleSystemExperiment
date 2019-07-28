using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTERingController : MonoBehaviour {

    public float Speed = 0.6f;

    public GameObject OuterCircle;
    public GameObject InnerCircle;

    public bool IsHit = false;
    public bool IsPressed = false;
    public bool IsHidden = false;

    public int NumberOfOccurences = 0;
    float scale = 1.0f;
    float MinScaleRange = 0.6f;

    static QTERingController instance;
    public static QTERingController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new QTERingController();
            }
            return instance;
        }
    }

    public void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Start()
    {
        HideQTE();
    }

    void Update () {

        if (IsHidden)
            return;

        if(Input.GetKeyDown(KeyCode.F) && !IsPressed)
        {
            IsPressed = true;

            if (OuterCircle.transform.localScale.x < MinScaleRange) //check max for extra difficulty?
                IsHit = true;
            else
                IsHit = false;

            if (IsHit)
            {
                if (NumberOfOccurences > 0)
                    GameManager.Instance.SpawnHealthPotion(); //Spawn random health potion on success except for the first time when the battle starts.

                //We succesfully did the QTE!
                scale = 1f;
                ChainBarDisplayController.Instance.AddToChainBar(10);

            }
            else
            {
                //we missed, remove the button prompt already
                InnerCircle.SetActive(false);
            }

            NumberOfOccurences++;
        }

        if(IsHit)
        {
            //We've done the QTE, animate it to exit
            scale += 3 * Time.deltaTime;
            OuterCircle.SetActive(false);

            InnerCircle.transform.localScale = new Vector3(scale, scale, scale);

            if (scale >= 2)
            {
                IsHidden = true;
                InnerCircle.SetActive(false);
            }

        }
        else
        {
            scale -= Speed * Time.deltaTime;

            OuterCircle.transform.localScale = new Vector3(scale, scale, scale);

            if (scale <= 0)
            {
                IsHidden = true;
                InnerCircle.SetActive(false);
                OuterCircle.SetActive(false);
            }
        }
		
	}

    public void ResetAndShowQTE()
    {
        scale = 1;
        IsPressed = false;
        InnerCircle.transform.localScale = new Vector3(1, 1, 1);
        OuterCircle.transform.localScale = new Vector3(1, 1, 1);

        InnerCircle.SetActive(true);
        OuterCircle.SetActive(true);
        IsHidden = false;
        IsHit = false;
    }

    public void HideQTE()
    {
        IsHidden = true;
        InnerCircle.SetActive(false);
        OuterCircle.SetActive(false);
    }
}
