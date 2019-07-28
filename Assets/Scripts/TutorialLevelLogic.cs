using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLevelLogic : MonoBehaviour {

    public bool IsDoorOpen = false;
    private static TutorialLevelLogic instance;
    public static TutorialLevelLogic Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TutorialLevelLogic();
            }
            return instance;
        }
    }

    void Awake()
    {
        // First we check if there are any other instances conflicting
        if (instance != null && instance != this)
        {
            Debug.Log("There's already a TutorialLevelLogic object in the scene, destroying this one.");
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            Debug.Log("TutorialLevelLogic woke");
        }
    }

    // Use this for initialization
    public SmoothMoveToPosition DoorChanger;

    public void OpenTutorialDoor()
    {
        EventQueue.Instance.AddAction(() => { DoorChanger.enabled = true; });
        IsDoorOpen = true;
    }
}
