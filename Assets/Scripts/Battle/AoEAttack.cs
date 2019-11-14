using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEAttack : MonoBehaviour {

    public GameObject Instigator;
    public float PreviewDuration = 3;
    public int Damage;
    public float TextureScrollSpeed = 0.6f;

    private float durationTimer = 0;

    private Renderer rend;
    public float SphereRange = 50;

    public bool IsRectangle = false;
    public GameObject SideA;
    public GameObject SideB;

    public bool IsFallingAoE = false;
    public float FallingHeight = 15;
    public float FallingSpeed = 3;

    void Start () {
        durationTimer = PreviewDuration;

        rend = GetComponent<Renderer>();

        if(IsFallingAoE)
            SetupFallingAoEMode();
    }

    void FallingAoECallback()
    {
        var intersectList = GetIntersects();

        foreach (var item in intersectList)
        {
            RPGActor actor = item.GetComponent<RPGActor>();
            if (actor.IsLeader)
                item.GetComponent<RPGActor>().ReceiveIncidentalDamage(4, MagicElemancy.None);
            CoreUIManager.Instance.SpawnLabel("AoE", GameManager.Instance.GetPartyLeader(), true);
        }

        GameObject.Destroy(this.gameObject);
        GameManager.Instance.ActiveAOEs--;
    }

    public void SetupFallingAoEMode()
    {
        if (GameManager.Instance.GetPartyLeader().GetComponent<RPGActor>().State == ActorState.Dead)
            return;

        SmoothMoveToPosition m = GetComponent<SmoothMoveToPosition>();
        this.transform.position = GameManager.Instance.GetPartyLeader().transform.position + new Vector3(0, FallingHeight, 0);
        m.Speed = FallingSpeed;
        m.StartSingleMoveAnimationToPosition(GameManager.Instance.GetPartyLeader().transform.position, FallingAoECallback);
        IsFallingAoE = true;
    }
	
	void Update () {

        if (IsFallingAoE)
            return;
        
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
            foreach (var actor in GetIntersects())
            {
                if(actor.GetComponent<RPGActor>().IsLeader)
                    Instigator.GetComponent<RPGActor>().DoDamageAttack(Damage, MagicElemancy.Fire, actor, 3f);
                else
                    Instigator.GetComponent<RPGActor>().DoDamageAttack(1, MagicElemancy.None, actor, 2f); //damage party members too but only a little

                CoreUIManager.Instance.SpawnLabel("AoE", actor, true);
            }
           
            GameObject.Destroy(this.gameObject);
            GameManager.Instance.ActiveAOEs--;
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

    public void CheckRectangleIntersect(GameObject Rectangle, ref List<GameObject> resultList)
    {
        Collider c = Rectangle.GetComponent<BoxCollider>();
        Collider[] hits = Physics.OverlapBox(c.bounds.center, c.bounds.size / 2);

        if (hits.Length > 0)
        {
            foreach (var col in hits)
            {
                if (col.tag == "Player")
                {
                    resultList.Add(col.gameObject);
                }
            }
        }
    }

    public List<GameObject> GetIntersects()
    {
        List<GameObject> results = new List<GameObject>();

        if (IsRectangle)
        {
            CheckRectangleIntersect(SideA, ref results);
            CheckRectangleIntersect(SideB, ref results);
        }
        else
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, SphereRange);
            if (hitColliders.Length > 0)
            {
                foreach (var col in hitColliders)
                {
                    if (col.tag == "Player")
                    {
                        results.Add(col.gameObject);
                    }
                }
            }

        }

        return results;
    }
}
