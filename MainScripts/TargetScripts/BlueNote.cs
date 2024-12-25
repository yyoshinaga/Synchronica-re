using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HitStatus = ScoreManager.HitStatus;  // Alias GameManager.HitStatus as HitStatus

public class BlueNote : GenericNote
{
    // Specific properties for BlueNote
    public const NoteType noteType = NoteType.BLUE; 

    // Accessible variables

    // Audio

    // Text
    
    public override void Start()
    {
        // Call base Start to initialize common properties
        base.Start();

        // Initialize specific properties for BlueNote
  
    }

    // Update is called once per frame
    public override void Update()
    {   
        // Call parent class Update
        base.Update();
    }
}