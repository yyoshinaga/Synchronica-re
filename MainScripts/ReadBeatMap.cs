using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System;

using Rank = SelectedSong.Rank;  // Alias SelectedSong.Rank as Rank

public class ReadBeatMap : MonoBehaviour
{
    private float delay; 

    // public ArrayList list;
    public List<NoteData> dataList;

    public AudioSource myAudio;


    public void Play()
    {
        myAudio.Play();
    }

    private void SetScore(Dictionary<Rank, int> scoreDict)
    {
        SelectedSong.SetRank(scoreDict);
    }

    public List<NoteData> Read()
    {   
        // Files must be within Resources folder
        string audioFilePath = SelectedSong.audioFilePath != null ? 
                                SelectedSong.audioFilePath : "Debug/audio";
        string beatmapFilePath = SelectedSong.beatMapFilePath != null ? 
                                SelectedSong.beatMapFilePath : "Debug/beatmap";

        // Load in osu beatmap
        myAudio = GetComponent<AudioSource>();
        myAudio.clip = Resources.Load<AudioClip>(audioFilePath);
        myAudio.Pause();
        TextAsset textFile = Resources.Load<TextAsset>(beatmapFilePath) as TextAsset;
        
        Debug.Assert(textFile != null, $"beatmap file {beatmapFilePath} could not get read");

        // Split file by newline
        string[] lines = textFile.text.Split('\n');

        // Get Score
        string lineWithScore = lines.FirstOrDefault(line => line.Contains("Score")).Split(':')[1];
        string[] scoreLine = lineWithScore.Split(',');
        
        // Extract score ranks and values
        Dictionary<Rank, int> ScoreDict = new Dictionary<Rank, int>();
        // TODO: Change this to while loop until [HitObjects]
        for (int i = 0; i < 7; i++)
        {
            string[] splitByDash = scoreLine[i].Split('-');

            string rank = splitByDash[0];
            int scoreValue = int.Parse(splitByDash[1]);
            ScoreDict.Add(SelectedSong.StringToRank(rank), scoreValue);
        }

        //Set the scores
        SetScore(ScoreDict);

        // Get delay
        string lineWithDelay = lines.FirstOrDefault(line => line.Contains("Delay"));
        delay = float.Parse(lineWithDelay.Split(':')[1]);

        // Find line with [HitObjects]
        int hitObjectsIdx = Array.FindIndex(lines, line => line.Trim() == "[HitObjects]");

        int counter = hitObjectsIdx+1;

        // Create variable to store output
        dataList = new List<NoteData>();

        // For loop starting after line [HitObjects]
        foreach (string line in lines.Skip(hitObjectsIdx+1))
        {   
            // Skip empty lines 
            if(string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            // Split each line by comma
            string[] splitByComma = line.Split(',');

            // Set declaration outside of try catch
            int x = 0, y = 0, time = 0; 
            NoteType noteType = NoteType.BLUE;

            try
            {
                x = int.Parse(splitByComma[0]);
                y = int.Parse(splitByComma[1]);
                time = int.Parse(splitByComma[2]);
                // Cast to enum
                noteType = (NoteType)int.Parse(splitByComma[3]);
            } 
            catch
            {
                Debug.LogError("1 Failed to convert string to int: " + line);
            }

            // Create a new note object
            NoteData note = new NoteData(x,y,time,noteType);

            switch (noteType)
            {
                case NoteType.BLUE:
                    break;
                case NoteType.RED:
                    // Get ending point and use that to determine direction
                    string[] parts = (splitByComma[5].Split('|')[1]).Split(':');
                    int endX = 0, endY = 0;
                    try
                    {
                        endX = int.Parse(parts[0]);
                        endY = int.Parse(parts[1]);
                    }
                    catch
                    {
                        Debug.LogError("2 Failed to convert string to int: " + parts);
                    }
                    float angle = Mathf.Atan2(endY - y, endX - x) * Mathf.Rad2Deg;
                    note.SetAngle(angle);
                    break;
                case NoteType.GREEN:
                    note.SetEndTime(FindEndTime(counter, x, y, lines));
                    break;
                case NoteType.YELLOW:
                    note.SetEndTime(FindEndTime(counter, x, y, lines));

                    string[] pointArr = splitByComma[5].Split('|');
                    List<int[]> p = new List<int[]>();
                    int[] firstPoint = new int[] { x, y };
                    p.Add(firstPoint);

                    // Skip first value since that is the letter (B,K,L)
                    for(int k = 1; k < pointArr.Length; k++)
                    {
                        p.Add(Array.ConvertAll(pointArr[k].Split(':'), int.Parse)); 
                    }
                    note.SetPoints(p);
                    break;
                default:
                    throw new Exception("ReadBeatMap, could not find note type");
            }

            // Set up links
            List<int[]> links = GetLinks(counter, lines);
            if (links.Count > 0)
            {
                note.hasLinks = true;
                note.SetLinks(links);
            }

            // Increment
            counter++;
            // Append note
            dataList.Add(note);
        }
        Debug.Log("Total number of notes: " + dataList.Count);

        return dataList;
    }

    private List<int[]> GetLinks(int idx, string[] lines)
    {
        // If end of liens or next line is empty, return blank list
        if (lines.Length == idx+1 || string.IsNullOrWhiteSpace(lines[idx+1]))
        {
            return new List<int[]>{};
        }

        string[] line = lines[idx].Split(',');
        int x = 0, y = 0, time = 0;
        try
        {
            x = int.Parse(line[0]);
            y = int.Parse(line[1]);
            time = int.Parse(line[2]);
        }
        catch
        {
            Debug.LogError("3 Failed to convert string to int: " + line);
            foreach (var l in line)
            {
                Debug.Log(l);
            }
        }

        // Check to see if the next note has the same timestamp
        List<int[]> links = new List<int[]>();
        string[] splitByComma = lines[idx+1].Split(',');

        int dx = 0, dy = 0, dt = 0;
        try
        {
            dx = int.Parse(splitByComma[0]);
            dy = int.Parse(splitByComma[1]);
            dt = int.Parse(splitByComma[2]);
        }
        catch
        {
            Debug.LogError("4 Failed to convert string to int: " + splitByComma);
            foreach (var l in splitByComma)
            {
                Debug.Log(l);
            }
            Debug.Log("splitBycomma: " + lines[idx+1]);
        }

        if (dt == time)
        {
            links.Add(new int[] { x, y } );
            links.Add(new int[] { dx, dy });
        }
        return links;
    }
     
    private int FindEndTime(int idx, int targetX, int targetY, string[] lines)
    {
        for (int i = idx; i < lines.Length; i++)
        {
            // Split each line by comma
            string[] line = lines[i].Split(',');

            int eX = int.Parse(line[0]);
            int eY = int.Parse(line[1]);
            int eTime = int.Parse(line[2]);

            if (eX == targetX && eY == targetY)
            {
                return eTime;
            }
        }

        string[] splitByComma = lines[idx].Split(',');
        int x = int.Parse(splitByComma[0]);
        int y = int.Parse(splitByComma[1]);
        int time = int.Parse(splitByComma[2]);
        
        string report = "Reporting that end to the slider at " + 
            time + " was not found. " + x + " " + y + " is the coordinate";
        throw new Exception(report);
    }

    public float GetDelay()
    {
        return delay;
    }
}
