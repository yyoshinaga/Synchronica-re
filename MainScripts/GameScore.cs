using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HitStatus = ScoreManager.HitStatus;  // Alias GameManager.HitStatus as HitStatus

// This class should only be accessed from ScoreManager
public static class GameScore 
{
    // Dictionary can't be constant
    public static readonly Dictionary<HitStatus, int> points = 
        new Dictionary<HitStatus, int>
        {
            { HitStatus.EARLY, 2 },
            { HitStatus.GREAT, 5},
            { HitStatus.PERFECT, 10 },
            { HitStatus.SLOW, 3 },
            { HitStatus.MISSED, 0}
        };

    public static Dictionary<HitStatus, int> hitStatusCounts = 
        new Dictionary<HitStatus, int>
        {
            { HitStatus.EARLY, 0 },
            { HitStatus.GREAT, 0},
            { HitStatus.PERFECT, 0 },
            { HitStatus.SLOW, 0 },
            { HitStatus.MISSED, 0}
        };

    public static int score = 0;
}
