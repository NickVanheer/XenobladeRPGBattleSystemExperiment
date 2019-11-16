using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AggroLineVisualizer : Track {

    public bool IsStraightLine = false;
    public int InterpolationSteps = 8;
    public float ControlPointOffsetY;

    public Color TargettingColor;
    public Color HasBeenTargettedColor;

    //Quadratic bezier
    private Vector3 startPoint; //p0
    private Vector3 controlPoint; //p1
    private Vector3 endPoint; //p2

    RPGActor actor;
    LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        if (line == null)
            line = transform.GetComponentInParent<LineRenderer>();
        if (line == null)
            line = this.gameObject.AddComponent<LineRenderer>();

        actor = GetComponent<RPGActor>();
        if (actor == null)
            actor = transform.GetComponentInParent<RPGActor>();

        Assert.IsNotNull(actor, "No RPGActor component could be found");
    }

    void Update ()
    {
        //For the enemies 
        if (actor.tag == "Enemy" && actor.State == ActorState.Engaged) //don't show aggro line from leader to enemy, only from enemy to leader. Otherwise there are two aggro lines.
        {
            line.enabled = true;
            line.startColor = HasBeenTargettedColor;
            line.endColor = HasBeenTargettedColor;

            SetupAggroLine(actor.gameObject, actor.TargetObject);
        }
        else if(actor.tag == "Enemy")
        {
            line.enabled = false;
        }
      
        //For the players when just targetting
        if (actor.tag == "Player" && /* actor.State == ActorState.Idle && */ actor.SoftTargetObject != null)
        {
            line.enabled = true;
            line.startColor = TargettingColor;
            line.endColor = TargettingColor;

            line.material.color = TargettingColor;
            SetupAggroLine(actor.gameObject, actor.SoftTargetObject);
        }
        else if(actor.tag == "Player")
        {
            line.enabled = false;
        }
	}

    public void SetupAggroLine(GameObject source, GameObject target)
    {
        //Starting points
        startPoint = source.transform.position;
        endPoint = target.transform.position;

        //
        Transform thisTopPoint = source.transform.FindDeepChild("ActorTopPoint");

        if (thisTopPoint != null)
            startPoint = thisTopPoint.transform.position;

        //
        Transform targetTopPoint = target.transform.FindDeepChild("ActorTopPoint");

        if (targetTopPoint != null)
            endPoint = targetTopPoint.transform.position;

        SetupAggroBezier(line, startPoint, endPoint);
    }

    public override void CalculateStartPoints()
    {
        ClearPoints();

        float step = 1.0f / (InterpolationSteps - 1); //if interstep = 5 [ 1 / 5 = 0.2, but steps should be 0.25 to be correct!!!]

        for (int i = 0; i < InterpolationSteps; ++i)
        {
            var t = i * step;
            var point = GetPointOnTrack(t);
            AddPoint(point);
        }
    }

    public override Vector3 GetPointOnTrack(float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 p = uu * startPoint + 2 * u * t * controlPoint + tt * endPoint;
        return p;
    }

    public void SetupAggroBezier(LineRenderer line, Vector3 startPoint, Vector3 endPoint)
    {
        controlPoint = (startPoint + endPoint) / 2;
        controlPoint += new Vector3(0, ControlPointOffsetY, 0);

        CalculateStartPoints();
        line.positionCount = ShapePoints.Count;

        for (int i = 0; i < ShapePoints.Count; i++)
        {
            line.SetPosition(i, ShapePoints[i]);
        }
    }

    public void SetupAggroStraightLine(LineRenderer line)
    {
        line.SetPosition(0, actor.transform.position);
        line.SetPosition(1, actor.TargetObject.transform.position);
    }
}
