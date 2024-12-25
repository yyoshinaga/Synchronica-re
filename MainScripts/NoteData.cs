using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteData 
{
    private int x;
    private int y;
    private int time;
    private NoteType noteType;
    private int endTime;
    private float angle;
    private List<int[]> points;
    
    public List<int[]> links;
    public bool hasLinks;
    public int skipCounter;
   
    public NoteData(int x, int y, int time, NoteType type)
    {
        this.x = x;
        this.y = y;
        this.time = time;
        SetNoteType(type);

        // Default
        hasLinks = false;
    }

    public int GetNumOfPoints()
    {
        return points.Count + 1;
    }

    public void SetAngle(float angle)
    {
        this.angle = angle;
    }
    public void SetEndTime(int endTime)
    {
        this.endTime = endTime;
    }
    public void SetPoints(List<int[]> points)
    {
        this.points = points;
    }
    public void SetLinks(List<int[]> links)
    {
        this.links = links;
    }

    private void SetNoteType(NoteType type)
    {
        noteType = type;
    }

    public NoteType GetNoteType()
    {
        return noteType;
    }

    public int GetX()
    {
        return x;
    }
    public int GetY()
    {
        return y;
    }
    public int GetTime()
    {
        return time;
    }
    public float GetAngle()
    {
        return angle;
    }
    public int GetEndTime()
    {
        return endTime;
    }
    public List<int[]> GetPoints()
    {
        return points;
    }
    public List<int[]> GetLinks()
    {
        return links;
    }
}