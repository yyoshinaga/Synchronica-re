using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HitStatus = ScoreManager.HitStatus;  // Alias GameManager.HitStatus as HitStatus

public class Projectile
{
    // Constants
    private const float prjDistance = 25f;
    private const float firingAngle = 80f;
    private const float gravity = 170f;
    private const float thresholdTime = 0.05f;
    private const float visibleVelocity = 40f;
    private const float velocityAtTarget = -55f;
    private const float velocityAtPerfectBuffer = 10f;
    private readonly Dictionary<HitStatus, float> hitStatusBounds = 
        new Dictionary<HitStatus, float>
    {
        { HitStatus.EARLY, -55f },  
        { HitStatus.GREAT, -82f },  
        { HitStatus.PERFECT, -18f }
    };
    private readonly Vector3 prjInitOffset = new Vector3(prjDistance, 0, 0);

    // Variables
    private bool isHitMissed;
    private float elapsedTime;
    private float initVelX;
    private float initVelY;
    private float totalFlightDuration;
    private bool isTargetReady;

    public Projectile(Transform noteTransform, Transform prjTransform)
    {
        // Initialize
        isHitMissed = false;
        elapsedTime = 0f;

        // Offset projectile from target
        prjTransform.position = new Vector3(noteTransform.position.x, 
                                            noteTransform.position.y, 
                                            noteTransform.position.z) + prjInitOffset;
        
        /* Derivation
            Vertical equation
            total distance covered is 0
            0 = v_i_y * t + 0.5*g*t^2
            t = -2 * v_i_y / g (solve for t)
            Plug t into horizontal equation
            d = v_i_x * (-2 * v_i_y / g)
            d = v_i*cos(theta) * (- 2 * v_i*sin(theta) / g)
            since sin(A + B) = sin(A)cos(B) + sin(B)cos(A)
            and sin(A + A) = sin(A)cos(A) + sin(A)cos(A) 
            = 2*sin(A)*cos(A) (double angle identity)
            v_i = sqrt(d * g / sin(2*theta))
        */

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float initPrjVelocity = 
            Mathf.Sqrt(prjDistance * gravity / Mathf.Sin((2 * firingAngle) * Mathf.Deg2Rad));

        // Extract the X  Y componenent of the velocity
        initVelX = initPrjVelocity * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        initVelY = initPrjVelocity * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        totalFlightDuration = prjDistance / initVelX;

        // Set flag to ready
        isTargetReady = true;
    }

    private float GetPrjVelocity()
    {
        // V_f = V_i + (acc * dt)
        return initVelY - (gravity * elapsedTime);
    }

    public void MoveProjectile(Transform prjTransform, float deltaTime)
    {
        // // Negative X velocity because projectile moves leftwards
        prjTransform.Translate(new Vector3(
            -initVelX * deltaTime, 
            GetPrjVelocity() * deltaTime,
            0)
        );
    }

    public void AddToElapsedTime(float time)
    {
        elapsedTime += time;
    }

    public bool HasLanded()
    {

        return elapsedTime > totalFlightDuration;
    }

    public bool IsAscending()
    {
        return GetPrjVelocity() > 0f;
    }

    public bool IsWithinTargetCenter()
    {
        float velocity = GetPrjVelocity();
        float lower = velocityAtTarget - velocityAtPerfectBuffer;
        float upper = velocityAtTarget + velocityAtPerfectBuffer;
        return lower < velocity && velocity < upper;
    }

    public bool IsPastTargetCenter(Transform prjTransform)
    {
        // Velocity is negative going downwards
        return -2f < prjTransform.localPosition.x && 
                    prjTransform.localPosition.x < 2f;
    }

    public bool IsReadyToAppear()
    {
        return GetPrjVelocity() < visibleVelocity;
    }

    public HitStatus CurrentHitStatus()
    {
        HitStatus hitStatus;

        if (isHitMissed)
        {
            hitStatus = HitStatus.MISSED;
        }
        else
        {
            float velocity = GetPrjVelocity();

            if (velocity <= hitStatusBounds[HitStatus.EARLY])
            {
                hitStatus = HitStatus.EARLY;
            }
            else if (velocity <= hitStatusBounds[HitStatus.GREAT])
            {
                hitStatus = HitStatus.GREAT;
            }
            else if (velocity <= hitStatusBounds[HitStatus.PERFECT])
            {
                hitStatus = HitStatus.PERFECT;
            }
            else
            {
                hitStatus = HitStatus.SLOW;
            }
        }
        return hitStatus;
    }

    public bool IsTargetReady()
    {
        return isTargetReady;
    }

    public void SetHitMissed()
    {   
        isHitMissed = true;
    }
}
