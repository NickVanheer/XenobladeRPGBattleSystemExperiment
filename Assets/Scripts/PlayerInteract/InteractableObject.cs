using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum InteractType
{
    ReturnKeyInteract, OnTriggerEnter
};

public class InteractableObject : MonoBehaviour, IInteractable
{
    public string TriggerToFire;

    [Header("Show object when player is near")]
    public GameObject Player;

    [Header("Properties")]
    public InteractType TypeOfInteraction;
    public List<string> AffectedByTag;
 
    public float DiscoverRange = 4;
    public UnityEvent ActionVariable;

    public bool IsIncreaseLightIntensity = true;
    public float NearbyLightIntensity = 5f;
    public float DefaultLightIntensity = 1f;

    public void Start()
    {
        if (Player == null)
            Player = GameManager.Instance.GetPartyLeader();

        DiscoverRange = 7f;
    }

    //Todo: reformat
    public void Update()
    {
        if (IsIncreaseLightIntensity)
        {
            float pos = Vector3.Distance(transform.position, Player.transform.position);

            if (IsIncreaseLightIntensity && Mathf.Abs(pos) < DiscoverRange)
                transform.GetChild(1).GetComponent<Light>().intensity = NearbyLightIntensity;
            else
                transform.GetChild(1).GetComponent<Light>().intensity = DefaultLightIntensity;
        }
    }
    public void Interact()
    {
        //if(TriggerToFire != "")
            //GameManager.Instance.FireGameFlowTrigger(TriggerToFire);

        if (ActionVariable != null)
            ActionVariable.Invoke();
    }

    void OnTriggerEnter(Collider col)
    {
        if (TypeOfInteraction != InteractType.OnTriggerEnter)
            return;

        if (AffectedByTag.Contains(col.gameObject.tag))
            Interact();
    }
}
