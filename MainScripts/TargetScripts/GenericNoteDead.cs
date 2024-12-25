using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GenericNoteDead : MonoBehaviour
{
    // protected Transform Target;
    // protected  Vector2 targetScale;

    protected enum TouchTypes {TOOEARLY, EARLY, PERFECT, LATE, TOOLATE};


    // protected Vector2 noteScale;
    protected float targetScaleStepsUp = 0.005f; //0.02f; //0.005
    protected float noteScaleStepsUp = 0.005f; //0.025f; //0.005
    protected float noteScaleDownSlow = 0.004f; //0.025f; //0.004
    protected float noteScaleDownRapid = 0.005f; //0.05f; //0.005
    protected float targetDistanceOffset = 10;

    //Variables
    // protected float visibleVariable = 50;
    protected float flipVariable = -55;
    protected float firingAngle = 80f;
    protected float gravity = 200f;

    // protected struct TouchTypes
    // {

    // }

    protected TouchTypes EvaluateTouch(float currentVelocity)
    {
        TouchTypes touchEval = TouchTypes.TOOEARLY;
        // < -10
        //-18 ~ -82

        if (currentVelocity >= flipVariable + 45 && 
            currentVelocity < flipVariable + 37)
        {
            touchEval = TouchTypes.EARLY;
        }
        else if (currentVelocity >= flipVariable + 37 && 
                currentVelocity < flipVariable - 27)
        {
            touchEval = TouchTypes.PERFECT;
        }
        else {
            touchEval = TouchTypes.LATE;
        }

        return touchEval;
    }



    protected struct ProjectileVariables
    {
        public bool targetIsReady;
        public float flightDuration;
        public float elapseTime;
        public float thresholdTime;
        public float comparisonDuration;
        public bool keepShrinking;  
        public float gravity;
        public float velX;
        public float velY;
    }

    protected ProjectileVariables ProjectileParameters(Vector3 notePosition)
    {
        // Calculate distance to target
        float targetDistance = 
            transform.InverseTransformPoint(notePosition).magnitude - targetDistanceOffset;                //Target.InverseTransformPoint(transform.position).magnitude; //Vector3.Distance(Projectile.position, targetPos);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectileVelocity = 
            Mathf.Sqrt(targetDistance * gravity / Mathf.Sin((2 * firingAngle) * Mathf.Deg2Rad));

        // Extract the X  Y componenent of the velocity
        float velX = projectileVelocity * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float velY = projectileVelocity * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = targetDistance / velX;
        float thresholdTime = 0.05f;

        ProjectileVariables pv = new ProjectileVariables();
        pv.targetIsReady = true;
        pv.flightDuration = flightDuration;
        pv.elapseTime = 0;
        pv.thresholdTime = thresholdTime;
        pv.comparisonDuration = flightDuration + thresholdTime + 0.7f;
        pv.keepShrinking = false;
        pv.gravity = gravity;
        pv.velX = velX;
        pv.velY = velY;
        return pv;
    }

    protected struct ScaleAndColor
    {
        public Vector2 scale;
        public Color color;
    }

    private ScaleAndColor CreateScaleAndColor(Vector2 scale, Color color) 
    {
        ScaleAndColor scaleAndColor = new ScaleAndColor();
        scaleAndColor.scale = scale;
        scaleAndColor.color = color;
        return scaleAndColor;
    }

    protected ScaleAndColor GrowTarget(Vector2 targetScale, Color targetColor)
    {
        targetScale.x += targetScaleStepsUp;
        targetScale.y += targetScaleStepsUp;
        targetColor.a += 0.1f;                 //Increase the opacity

        return CreateScaleAndColor(targetScale, targetColor);
    }


    protected ScaleAndColor GrowNote(Vector2 noteScale, Color noteColor)
    {
        noteScale.x += noteScaleStepsUp;
        noteScale.y += noteScaleStepsUp;
        noteColor.a += 0.1f;                 //Increase the opacity

        return CreateScaleAndColor(noteScale, noteColor);
    }

    protected Vector2 GrowNoteInvisible(Vector2 noteScale)
    {
        noteScale.x += noteScaleStepsUp;
        noteScale.y += noteScaleStepsUp;
        return noteScale;
    }


    protected Vector2 ShrinkNoteSlow(Vector2 noteScale)
    {
        noteScale.x = Mathf.Max(noteScale.x - noteScaleDownSlow, 0);
        noteScale.y = Mathf.Max(noteScale.y - noteScaleDownSlow, 0);
        return noteScale;
    }

    protected ScaleAndColor ShrinkNoteRapid(Vector2 noteScale, Color noteColor)
    {
        if (noteScale.x > 0.01f) {
            noteScale.x = Mathf.Max(noteScale.x-noteScaleDownRapid, 0);
            noteScale.y = Mathf.Max(noteScale.y-noteScaleDownRapid, 0);
        }
        else {
            noteColor.a = 0.0f;
        }

        return CreateScaleAndColor(noteScale, noteColor);
    }

    protected Vector2 SlightlyScaleDownTarget(Vector2 targetScale)
    {
        targetScale.x -= 0.05f;
        targetScale.y -= 0.05f;
        return targetScale;
    }

    protected ScaleAndColor MakeNoteMoveOrDisappear(bool isPerfect, Vector2 noteScale, Color noteColor)
    {
        if (isPerfect)
        {
            noteScale = Vector2.zero;
        }
        else
        {
            noteColor.a = 0f;                 //Make note disappear
        }
        return CreateScaleAndColor(noteScale, noteColor);
    }
}