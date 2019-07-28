using UnityEngine;
using System.Collections;

public class CameraChangeTrigger : MonoBehaviour {

    //unfinished
    public static int TriggerCount = 0;
    public int TriggerIndex = 0;

    public float NewYValue;
    public Vector3 OffsetIncrement;

    public bool IsHideMeshRenderer = true;
    public bool IsNewCamera = false;

    public Camera NewCamera;
    public Camera MainCamera;

    public CameraChangeTrigger ActivateOnHit;
    public bool IsStartTrigger = false;

    public float NewCameraHeight = 0;

	// Use this for initialization
	void Start () {
        TriggerCount++;
        TriggerIndex = TriggerCount;

        if (IsNewCamera && NewCamera != MainCamera)
            NewCamera.enabled = false;

        if (IsStartTrigger)
        {

            ActivateOnHit.gameObject.GetComponent<CameraChangeTrigger>().OffsetIncrement = new Vector3(-this.OffsetIncrement.x, -this.OffsetIncrement.y, -this.OffsetIncrement.z);
            ActivateOnHit.gameObject.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {

        if(IsHideMeshRenderer)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
	}

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.tag != "Player")
            return;

        if (!this.gameObject.activeInHierarchy)
            return;

        if (collider.gameObject != GameManager.Instance.GetPartyLeader())
            return;

            if (IsNewCamera)
            {
                Camera.main.enabled = false;
                NewCamera.enabled = true;
            }
            else
            {
         
                if (ActivateOnHit != null)
                {
                    ActivateOnHit.gameObject.SetActive(true);
                    ActivateOnHit.gameObject.GetComponent<CameraChangeTrigger>().NewCameraHeight = Camera.main.GetComponent<TestCam>().Offset.y;
                    this.gameObject.SetActive(false);

                TestCam c = Camera.main.GetComponent<TestCam>();
                c.RaiseCameraHeight(NewCameraHeight);
            }
            }
    }
}
