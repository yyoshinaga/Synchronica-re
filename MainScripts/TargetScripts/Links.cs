using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Links : MonoBehaviour
{
    public Vector2 point1;
    public Vector2 point2;
    public bool hasBeenHit;
    private int SEGMENT_COUNT;
    private LineRenderer lineRenderer;
    private Vector2[] linePixelArray;

    private Vector2 lineScale;
    private Color lineColor;
    


    // Start is called before the first frame update
    void Start()
    {
        SEGMENT_COUNT = 50;

        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        lineRenderer.material.SetColor("_Color", Color.green);

        lineScale = transform.localScale;
        //lineColor = GetComponent<SpriteRenderer>().color;
        DrawLine();
    }

    void Disappear()
    {
        if (lineScale.x > 0.0f)
        {
            Debug.Log("CEOMS HERE");
            /*
            lineScale.x -= 0.05f;
            lineScale.y -= 0.05f;
            transform.localScale = lineScale;
 
            lineColor.a = lineColor.a - 0.1f;                 //Increase the opacity
            transform.parent.GetComponent<SpriteRenderer>().color = lineColor;
            */
        }
    }

    void DrawLine()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, point1);
        lineRenderer.SetPosition(1, point2);

        Vector2 g = point1;
        Vector2 incre = (point2 - point1) / SEGMENT_COUNT;

        int linePixelArrayCounter = 0;
        linePixelArray = new Vector2[SEGMENT_COUNT];

        for (int i = 1; i <= SEGMENT_COUNT; i++)
        {
            linePixelArray[linePixelArrayCounter] = g + incre * i;
            linePixelArrayCounter++;
        }
    }
}