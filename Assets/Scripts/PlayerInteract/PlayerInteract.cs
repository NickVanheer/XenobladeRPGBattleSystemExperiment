using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact();
}

/*
public enum InteractType
{
    Normal, CustomFunction
}

*/

public class PlayerInteract : MonoBehaviour {

    public float InteractDistance = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (GameManager.Instance.IsPausedForUI)
                return;

            if (GameManager.Instance.GetPartyLeader().GetComponent<RPGActor>().State != ActorState.Idle)
                return;

            if (GameManager.Instance.GetPartyLeader().GetComponent<PlayerTargetNearest>().HasSoftTarget)
                return;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, InteractDistance);
            int i = 0;
            while (i < hitColliders.Length)
            {
                if(hitColliders[i].gameObject.tag == "Interactable")
                {
                    var _interactableObject = hitColliders[i].gameObject.GetComponent<IInteractable>();
                    if (_interactableObject != null)
                        _interactableObject.Interact();
                }

                i++;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, InteractDistance);
    }
}
