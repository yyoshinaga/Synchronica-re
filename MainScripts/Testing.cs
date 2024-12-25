using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(LineRenderer))]
public class Testing : MonoBehaviour
{
    public Transform[] curvePoints;
    private Transform[] controlPoints;
    public LineRenderer lineRenderer;

    private int curveCount = 0;
    private int layerOrder = 0;
    private int SEGMENT_COUNT = 50;


    void Start()
    {
       

        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.sortingLayerID = layerOrder;

        //curveCount = (int)controlPoints.Length / 3;
        curveCount = curvePoints.Length-1;
        controlPoints = new Transform[(curvePoints.Length-1)*3+1];

        //Curve points length = 3
        for(int i = 0; i < curvePoints.Length; i++)
        {
            controlPoints[i * 3] = curvePoints[i];
        }
        for(int i = 0; i < controlPoints.Length; i++)
        {
            if (i % 3 != 0)
            {
                controlPoints[i] = new GameObject().transform;
            }
        }

        Debug.Log(controlPoints.Length);
    }
    void Update()
    {
        DrawCurve();
    }

    void DrawCurve()
    {
        //j is controlPoints/3
        for (int j = 1; j < curveCount; j++)
        {
            int nodeIndex = j * 3;
            Vector2 dir = Vector2.zero;

            //Left Point:
            Vector2 offsetL = controlPoints[nodeIndex - 3].position - controlPoints[nodeIndex].position;
            dir += offsetL.normalized;
            float leftNeighbourDistances = offsetL.magnitude;

            //Right Point:
            Vector2 offsetR = controlPoints[nodeIndex + 3].position - controlPoints[nodeIndex].position;
            dir -= offsetR.normalized;
            float rightNeighbourDistances = -offsetR.magnitude;

            dir.Normalize();


            controlPoints[nodeIndex - 1].position = (Vector2)(controlPoints[nodeIndex].position) + dir * leftNeighbourDistances * .5f;
            controlPoints[nodeIndex + 1].position = (Vector2)(controlPoints[nodeIndex].position) + dir * rightNeighbourDistances * .5f;


        }

        //DO THE FIRST SET OF POINTS
        controlPoints[1].position = (Vector2) (controlPoints[0].position + controlPoints[2].position) * .5f;

        //DO THE LAST SET OF POINTS
        int max = controlPoints.Length;
        controlPoints[max - 2].position = (Vector2) (controlPoints[max - 1].position + controlPoints[max - 3].position) * .5f;
          

        for (int j = 0; j < curveCount; j++)
        {
            for (int i = 1; i <= SEGMENT_COUNT; i++)
            {
                float t = i / (float)SEGMENT_COUNT;
                int nodeIndex = j * 3;

                Vector3 pixel = CalculateCubicBezierPoint(t, controlPoints[nodeIndex].position, controlPoints[nodeIndex + 1].position, controlPoints[nodeIndex + 2].position, controlPoints[nodeIndex + 3].position);
                lineRenderer.positionCount = (j * SEGMENT_COUNT) + i;
                lineRenderer.SetPosition((j * SEGMENT_COUNT) + (i - 1), pixel);
            }
        }
    }

    Vector2 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }
}