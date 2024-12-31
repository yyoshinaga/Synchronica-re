using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ScoreManager
{
    public enum HitStatus { EARLY, GREAT, PERFECT, SLOW, MISSED }

    public static void AddScore(HitStatus hitStatus)
    {
        // Get the point value with HitStatus enum
        int points = GameScore.points[hitStatus];

        // Add points to score variable
        GameScore.score += points;
    }

    // Its best not to use this function unless for debug
    public static void SetScore(HitStatus hitStatus, int amount)
    {
        GameScore.hitStatusCounts[hitStatus] = amount;

        // Get the point value with HitStatus enum
        int points = GameScore.points[hitStatus];

        // Add points to score variable
        GameScore.score += points * amount;
    }

    public static int GetScore()
    {
        return GameScore.score;
    }

    public static int CalcHitStatusPoints(HitStatus hitStatus)
    {
        return GameScore.hitStatusCounts[hitStatus] * GameScore.points[hitStatus];
    }
}
