using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEAttack : MonoBehaviour {

    public GameObject Instigator;
    public float PreviewDuration = 3;
    public int Damage;
    public float TextureScrollSpeed = 0.6f;

    private float durationTimer = 0;
    private List<GameObject> foundActors;
    private Renderer rend;
    public float SphereRange = 50;

    public bool IsRectangle = false;
    public GameObject SideA;
    public GameObject SideB;

    void Start () {
        durationTimer = PreviewDuration;
        foundActors = new List<GameObject>();

        rend = GetComponent<Renderer>();
    }
	
	void Update () {

        //visual
        float offset = Time.time * TextureScrollSpeed;
        if (IsRectangle)
        {
            SideA.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0, offset));
            SideB.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0, offset));
        }
        else
        {
            GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0, offset));
        }

        if(Instigator == null)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        if (durationTimer <= 0 && Instigator != null)
        {
            CheckIntersect();
            foreach (var actor in foundActors)
            {
                Instigator.GetComponent<RPGActor>().DoDamageAttack(Damage, actor, 3f);
                CoreUIManager.Instance.SpawnLabel("AoE", actor, true);
            }
           
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            durationTimer -= Time.deltaTime;
        }
    }

    public void Scale(float increment)
    {
        this.transform.localScale *= increment;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SphereRange);
    }

    public void CheckRectangleIntersect(GameObject Rectangle)
    {
        Collider c = Rectangle.GetComponent<BoxCollider>();
        Collider[] hits = Physics.OverlapBox(c.bounds.center, c.bounds.size / 2);

        if (hits.Length > 0)
        {
            foreach (var col in hits)
            {
                if (col.tag == "Player" && col.GetComponent<RPGActor>().IsLeader)
                {
                    foundActors.Add(col.gameObject);
                }
            }
        }
    }

    public void CheckIntersect()
    {
        if(IsRectangle)
        {
            CheckRectangleIntersect(SideA);
            CheckRectangleIntersect(SideB);
        }
        else
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, SphereRange);
            if (hitColliders.Length > 0)
            {
                foreach (var col in hitColliders)
                {
                    if (col.tag == "Player" && col.GetComponent<RPGActor>().IsLeader)
                    {
                        Debug.Log("Player found");
                        foundActors.Add(col.gameObject);
                    }
                }
            }
        }
    }
}
