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
    public GameObject ObjectToShowWhenPlayerIsNear;
    public GameObject Player;

    [Header("Properties")]
    public InteractType TypeOfInteraction;
    public List<string> AffectedByTag;
    public bool IsAlwaysShowInteractionDisplay = false;
    public bool IsInteractionDisplayGoneAfterFirstInteract = false;

    public UnityEvent ActionVariable;

    public void Update()
    {
        if (ObjectToShowWhenPlayerIsNear != null)
        {
            float pos = Vector3.Distance(transform.position, Player.transform.position);

            if(Mathf.Abs(pos) < 7 || IsAlwaysShowInteractionDisplay)
            {
                ObjectToShowWhenPlayerIsNear.SetActive(true);
            }
            else
            {
                ObjectToShowWhenPlayerIsNear.SetActive(false);
            }

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
