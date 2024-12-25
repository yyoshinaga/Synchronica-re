using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class SelectedSong
{
    public enum Rank { S, A, B, C, D, E, F }
    // The songID is a unique integer
    public static int songID {get; set; }
    public static string songTitle { get; set; }
    public static string songFolder { get; set; }
    public static string beatMapFilePath { get; set; }
    public static string audioFilePath { get; set; }
    public static string genre {get; set; } // TODO: create enums? 
    public static string backgroundFile { get; set; }
    public static string difficulty { get; set; }
    public static float delay { get; set; }
    public static bool enableBackground {get; set; }
    public static bool enableHitSound { get; set; }

    // Contains no rank F because F is everything below E
    private static Dictionary<Rank, int> rankToValue = new Dictionary<Rank, int>
        {
            { Rank.S, 0 },
            { Rank.A, 0 },
            { Rank.B, 0 },
            { Rank.C, 0 },
            { Rank.D, 0 },
            { Rank.E, 0 }
        };

    private static readonly Dictionary<Rank, string> rankToLetter = new Dictionary<Rank, string>
        {
            { Rank.S, "S" },
            { Rank.A, "A" },
            { Rank.B, "B" },
            { Rank.C, "C" },
            { Rank.D, "D" },
            { Rank.E, "E" },
            { Rank.F, "F" }
        };

    private static Dictionary<string, Rank> letterToRank = new Dictionary<string, Rank>();

    public static Rank StringToRank(string letter)
    {   
        // Apply reverse mapping only if dic is not initialized
        if (letterToRank.Count == 0)
        {
            foreach (var entry in rankToLetter)
            {
                letterToRank.Add(entry.Value, entry.Key);  // Swap key and value
            }
        }
        // Check with uppercase letter only
        return letterToRank[letter.ToUpper()];
    }
    public static Rank GetRank(int points)
    {
        Rank result = Rank.F;

        // Use the enum as an ordered list to retrieve dictionary values
        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
        {   
            // If points are greater than the threshold,
            if (rankToValue[rank] <= points)
            {   
                // Set result to this rank
                result = rank;
            }
        }
        return result;
    }

    public static string RankToString(Rank rank)
    {
        return rankToLetter[rank];
    }

    public static void SetRank(Dictionary<Rank, int> dict)
    {
        rankToValue = dict;
    }

    public static void SetBackgroundFile(string file)
    {
        backgroundFile = file;
    }
}