using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HitStatus = ScoreManager.HitStatus;  // Alias GameManager.HitStatus as HitStatus

public class YellowNote : GenericNote
{
    // Specific properties for YellowNote
    public const NoteType noteType = NoteType.YELLOW; 

    // Accessible variables
    public List<int[]> controlPoints;
    public float holdLength;
    // public TextHandler textHandler; // Initialized in SongManager


    // Start is called before the first frame update
    public override void Start()
    {
        // Call base Start to initialize common properties
        base.Start();
        // textHandler = GetComponent<TextHandler>();

    }

    // Update is called once per frame
    public override void Update()
    {
        // Call parent class Update
        base.Update();
    }
}
