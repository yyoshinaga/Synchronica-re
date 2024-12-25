using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TouchIndicator
{
    public int touchId;
    public GameObject circle;

    public TouchIndicator(int newTouchId, GameObject newCircle)
    {
        touchId = newTouchId;
        circle = newCircle;
    }
}