using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargetNearest : MonoBehaviour {

    public float Range = 150;
    public bool HasSoftTarget = false;
    List<GameObject> targets = new List<GameObject>();
    int tIndex = 0;
    int totalFound = 0;

	void Update () {

        //Disengage soft target when pressing escape.
        if (GetComponent<RPGActor>().State == ActorState.Idle)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClearTargetSelection();
            }
        }

        if(HasSoftTarget)
        {
            //check distance and untarget when too far away
            GameObject enemy = GetComponent<RPGActor>().SoftTargetObject;

            if(enemy != null)
            {
                float distance = Vector3.Distance(enemy.transform.position, this.transform.position);
                if (Mathf.Abs(distance) > enemy.GetComponent<RPGActor>().DisengageDistance)
                {
                    ClearTargetSelection();
                    return;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            List<GameObject> found = GetComponent<RPGActor>().GetNearestEnemiesForPlayer(Range);

            if (found.Count <= 0)
                return;

            //loop backwards and destroy eventual dead references
            for (int i = targets.Count - 1; i >= 0; i--)
            {
                if (targets[i] == null)
                    targets.RemoveAt(i);
            }

            //We've found some potential enemies, 
            //The amount of new enemies is different from what was there before refresh target list.
            if (found.Count != totalFound)
            {
                targets.Clear();
                foreach (GameObject gO in found)
                {
                    targets.Add(gO);
                }
                totalFound = targets.Count;

                tIndex = 0;
            }
            else
            {
                tIndex++;

                if (tIndex > targets.Count - 1)
                    tIndex = 0;
            }

            SoftTarget();
        }
    }

    void SoftTarget()
    {
        if (targets[tIndex] == null)
            return; 

        GetComponent<RPGActor>().SetSoftTarget(targets[tIndex]);

        CoreUIManager.Instance.ShowTargetDisplay(GetComponent<RPGActor>().SoftTargetObject);
        CoreUIManager.Instance.ShowSkillDisplay();

        HasSoftTarget = true;
    }

    public void ClearTargetSelection()
    {
        HasSoftTarget = false;
        tIndex = 0;
        totalFound = 0;
        targets.Clear();
        GetComponent<RPGActor>().UnsetSoftTarget();

        CoreUIManager.Instance.HideTargetDisplay();
        CoreUIManager.Instance.HideSkillDisplay();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}
