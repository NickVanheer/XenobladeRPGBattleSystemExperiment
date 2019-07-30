using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AggroLineVisualizer : Track {

    public bool IsStraightLine = false;
    public int InterpolationSteps = 8;
    public float ControlPointOffsetY;
    public Vector3 ActorPointOffsetSource;
    public Vector3 ActorPointOffsetDestination;

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

            if (IsStraightLine)
                SetupAggroStraightLine(line);
            else
                SetupAggroBezier(line, actor.TargetObject.transform.position);
        }
        else
        {
            line.enabled = false;
        }
      
        //For the players
        if (actor.tag == "Player" && actor.State == ActorState.Idle && actor.SoftTargetObject != null)
        {
            line.enabled = true;
            line.startColor = TargettingColor;
            line.endColor = TargettingColor;

            line.material.color = TargettingColor;
            SetupAggroBezier(line, actor.SoftTargetObject.transform.position);
        }
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

    public void SetupAggroBezier(LineRenderer line, Vector3 targetPosition)
    {
        startPoint = actor.transform.position + ActorPointOffsetSource;
        endPoint = targetPosition + ActorPointOffsetDestination;
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
