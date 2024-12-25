using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using HitStatus = ScoreManager.HitStatus;  // Alias GameManager.HitStatus as HitStatus

public class GenericNote : MonoBehaviour
{
    // Note properties
    protected bool hasBeenHit;
    protected HitStatus? assignedHitStatus = null; // Set to null
    protected HitStatus currentHitStatus;
    protected bool hasNoteFinished;

    // Projectile
    protected Transform prjTransform;
    protected bool movePrjToTargetCenter;
    protected Projectile prj;
    protected SpriteRenderer prjSprite;

    // Target
    protected Transform targetTransform;
    protected SpriteRenderer targetSprite;

    // Ring
    private Transform ringTransform;
    private SpriteRenderer ringSprite;
    private Color ringColor;
    protected Dictionary<HitStatus, Color> selectRingColor = 
        new Dictionary<HitStatus, Color>
        {
            { HitStatus.EARLY, Color.grey },
            { HitStatus.GREAT, Color.cyan},
            { HitStatus.PERFECT, Color.white },
            { HitStatus.SLOW, Color.grey },
            { HitStatus.MISSED, Color.grey }
        };

    //Link
    public GameObject link;
    private LineRenderer linkRenderer;

    // Audio
    protected AudioSource audioSource;

    // Text
    public TextHandler textHandler; // Initialized in SongManager


    public virtual void Start()
    {
        // Allow multitouch
        Input.multiTouchEnabled = true;

        // Initialize common properties
        currentHitStatus = HitStatus.EARLY;
        hasNoteFinished = false;
        hasBeenHit = false;

        // Initialize Target
        targetTransform = transform.Find("Target");
        Debug.Assert(targetTransform != null, "targetTransform is null");

        targetSetScale(); // Must be called after transform is set
        targetSprite = targetTransform.GetComponent<SpriteRenderer>();
        targetOpacityToZero();

        // Initialize projectile
        prjTransform = transform.Find("Projectile");
        prjSprite = prjTransform.GetComponent<SpriteRenderer>();
        prjOpacityToZero();

        // Initialize Ring
        ringTransform = transform.Find("Ring");
        ringSprite = ringTransform.GetComponent<SpriteRenderer>();
        ringOpacityToZero();

        if (link != null)
        {
            linkRenderer = link.GetComponent<LineRenderer>();
            linkOpacityToZero();
        }
        
        // Set up audio
        audioSource = transform.GetComponent<AudioSource>();

        // Initialize projectile
        prj = new Projectile(transform, prjTransform);
        
        // Start the projectile movement
        StartCoroutine(SimulateProjectile());
    }

    // Abstract method for subclasses to implement for projectile simulation
    protected virtual IEnumerator SimulateProjectile()
    { 
        // Scale up target until it reaches max size
        yield return InitialTargetAnimation();

        // Scale up projectile and become visible while moving
        yield return ProjectileAscend();

        // Scale down projectile and disappear
        yield return ProjectileDescent();

        // Start making projectile disappear
        yield return ClearProjectile();

        // Destroy note
        yield return DestroyNote();
    }
    
    protected virtual IEnumerator InitialTargetAnimation()
    {
        Debug.Log("InitialTargetAnimation");

        // Grow target
        while (!isTargetMaxScale())
        {
            // Scale up the target object
            targetIncreaseScaleFast();

            // Increase opacity of target object
            targetIncreaseOpacity();

            // Apply animation at each frame
            yield return null;
        }
    }
    
    protected virtual IEnumerator ProjectileAscend()
    {        
        Debug.Log("projectile ascend");

        // If projectile is ascending, hasn't been hit and hasn't landed
        while (!hasBeenHit && !prj.HasLanded() && prj.IsAscending())
        {
            // Scale up the projectile object
            prjIncreaseScale();

            // If projectile is ready to become visible
            if (prj.IsReadyToAppear())
            {
                // Increase opacity of projectile object
                prjIncreaseOpacity();
            }

            // Update location of projectile based on time
            prj.MoveProjectile(prjTransform, Time.deltaTime);

            // Update elapse time for projectile
            prj.AddToElapsedTime(Time.deltaTime);

            // Yield return null to continue in the next frame
            yield return null;
        }
    }
    
    protected virtual IEnumerator ProjectileDescent()
    {        
        Debug.Log("projectile descent ");

        // If projectile hasn't been hit and hasn't landed,
        while (!hasBeenHit && !prj.HasLanded())
        {
            // Touch is only evaluated when projectile is falling
            currentHitStatus = prj.CurrentHitStatus();

            // Scale down the projectile object slowly
            prjDecreaseScaleSlow();

            // If projectile is at the center of the target,
            if (prj.IsWithinTargetCenter())
            {
                movePrjToTargetCenter = true;
            }

            // If projectile is past the target center
            if (prj.IsPastTargetCenter(prjTransform))
            {
                // Move projectile behind target
                prjSprite.sortingOrder = -1;
            }

            // Update location of projectile based on time
            prj.MoveProjectile(prjTransform, Time.deltaTime);

            // Update elapse time for projectile
            prj.AddToElapsedTime(Time.deltaTime);

            // Yield return null to continue in the next frame
            yield return null;
        }
    }
    
    protected virtual IEnumerator ClearProjectile()
    {
        Debug.Log("Clear projectile");

        // No longer need link
        if (link != null)
        {
            Destroy(link);
        }

        // While the sprite has not disappeared,
        while (!hasBeenHit && prjSprite.color.a > 0)
        {
            // Scale down the projectile object fast
            prjDecreaseScaleFast();
            
            // If scale is less than 0.01, make it transparent
            if (prjTransform.localScale.x <= 0.01f && 
                prjTransform.localScale.y <= 0.01f)
            {
                prjOpacityToZero();
            }

            // Yield return null to continue in the next frame
            yield return null;
        }
    }
    
    protected virtual IEnumerator DestroyNote()
    {
        Debug.Log("DestroyNote");

        // Turn off collider to avoid disturbing other notes underneath
        transform.GetComponent<CircleCollider2D>().enabled = false;
        
        if (hasBeenHit)
        {
            // Apply ring color
            // assignedHitStatus should never be null
            try 
            {
                if (assignedHitStatus != null)
                {
                    switch(assignedHitStatus)
                    {
                        case HitStatus.EARLY:
                            ringColor = selectRingColor[HitStatus.EARLY];
                            break;
                        case HitStatus.GREAT:
                            ringColor = selectRingColor[HitStatus.GREAT];
                            break;
                        case HitStatus.PERFECT:
                            ringColor = selectRingColor[HitStatus.PERFECT];
                            break;
                        case HitStatus.SLOW:
                            ringColor = selectRingColor[HitStatus.SLOW];
                            break;
                        case HitStatus.MISSED:
                            ringColor = selectRingColor[HitStatus.MISSED];
                            break;
                    }
                }
            }
            catch
            {
                Debug.LogError("assignedHitStatus was null");
            }
        }
        else
        {
            // Update current hit status to Missed
            prj.SetHitMissed();
            currentHitStatus = prj.CurrentHitStatus();
            assignedHitStatus = currentHitStatus;

            // Set projectile sprite opacity to zero
            prjOpacityToZero();
        }

        yield return DisappearingSequence();
    }

    protected virtual IEnumerator DisappearingSequence()
    {
        Debug.Log("DisappearingSequence");

        //Destruction phase
        while (targetSprite.color.a > 0)
        {
            // Decrease opacity of target object
            targetDecreaseOpacity();

            // Decrease opacity of projectile object
            prjDecreaseOpacity();
            
            if (hasBeenHit)
            {
                // Scale up the target object slightly
                targetIncreaseScaleSlow();

                // Scale up the ring object slightly
                ringIncreaseScale();

                if (ringColor.a < 1)
                {
                    // Increase opacity of ring object
                    ringIncreaseOpacity();
                }
                else if (!isRingMaxScale())
                {
                    // Decrease opacity of ring object
                    ringDecreaseOpacity();
                }
                else
                {
                    // Make ring disappear
                    ringOpacityToZero();
                }
            }
            yield return null;
        }

        // Wait a few seconds to allow text to appear
        yield return new WaitForSeconds(3);
 
        // After all animation, destroy the note object
        Destroy(gameObject);
    }

    public virtual void Update()
    {   
        // If the note is finished, don't do anything else
        if (hasNoteFinished)
        {
            return;
        }

        // Note is considered have been hit if not null nor missed
        hasBeenHit = assignedHitStatus != HitStatus.MISSED && 
                        assignedHitStatus != null;

        // If note is considered a miss and has not been hit yet,
        if (assignedHitStatus == HitStatus.MISSED)
        {   
            // Perform score and text management
            EvaluateHit();

            // Label as finished to prevent further updates
            hasNoteFinished = true;
        }
        else 
        {
            // If note has not finished its trajectory, check touches
            for (int i = 0; i < Input.touchCount; i++)
            {
                // Get touch object
                Touch touch = Input.GetTouch(i);

                // Get touch position
                Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
                RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

                // If person did not touch a note or target is not ready
                if (hit.collider == null || !prj.IsTargetReady())
                {
                    continue;
                }

                // Check if touch ID is equal to the note ID
                if (hit.collider.transform.GetInstanceID() == 
                        targetTransform.GetInstanceID())  //Check to see if touch is colliding with target
                {
                    // Set the assignedHitStatus to current hit status
                    assignedHitStatus = currentHitStatus;

                    // Play audio if not played yet
                    if (SelectedSong.enableHitSound)
                    {
                        audioSource.PlayOneShot(audioSource.clip, 1f);
                    }
                    
                    // Move projectile to target center
                    if (movePrjToTargetCenter)
                    {
                        prjTransform.position = Vector2.zero;
                    }
                    // Otherwise make opacity zero
                    else
                    {
                        prjOpacityToZero();
                    } 

                    // Scale down target slightly when touched
                    // This happens instantly since it doesn't not yield
                    targetDecreaseScaleSlow();

                    // Perform score and text management
                    EvaluateHit();
                    
                    // Label as finished to prevent further updates
                    hasNoteFinished = true;
                }
            }
        }
    }

    protected virtual void EvaluateHit()
    {
        // Handle scoring based on touch evaluation
        switch (assignedHitStatus)
        {
            case HitStatus.EARLY:
                textHandler.HandleEarlyText(transform.position);
                ScoreManager.AddScore(HitStatus.EARLY);
                break;
            case HitStatus.GREAT:
                textHandler.HandleGreatText(transform.position);
                ScoreManager.AddScore(HitStatus.GREAT);
                break;
            case HitStatus.PERFECT:
                textHandler.HandlePerfectText(transform.position);
                ScoreManager.AddScore(HitStatus.PERFECT);
                break;
            case HitStatus.SLOW:
                textHandler.HandleSlowText(transform.position);
                ScoreManager.AddScore(HitStatus.SLOW);
                break;
            case HitStatus.MISSED:
                textHandler.HandleMissedText(transform.position);
                ScoreManager.AddScore(HitStatus.MISSED);
                break;
        }
    }

    private bool isTargetMaxScale()
    {
        return NoteAnimation.IsMaxScale(targetTransform, NoteAnimation.targetMaxScale);
    }
    
    private void targetOpacityToZero()
    {
        NoteAnimation.SetOpacityToZero(targetSprite);
    }

    private void targetDecreaseOpacity()
    {
        NoteAnimation.ChangeOpacity(
            sprite: targetSprite,
            amount: NoteAnimation.targetDisappearAmount
        );
    }

    private void targetIncreaseOpacity()
    {
        NoteAnimation.ChangeOpacity(
            sprite: targetSprite,
            amount: NoteAnimation.targetAppearAmount
        );
    }

    private void targetDecreaseScaleSlow()
    {
        NoteAnimation.ChangeScale(
            transform: targetTransform, 
            amount: NoteAnimation.targetScaleDownSlightAmount
        ); 
    }

    private void targetIncreaseScaleSlow()
    {
        NoteAnimation.ChangeScale(
            transform: targetTransform, 
            amount: NoteAnimation.targetScaleUpSlightAmount
        ); 
    }

    private void targetIncreaseScaleFast()
    {
        NoteAnimation.ChangeScale(
            transform: targetTransform, 
            amount: NoteAnimation.targetScaleUpAmount
        );
    }

    private void targetSetScale()
    {
        NoteAnimation.SetScale(targetTransform, NoteAnimation.targetInitScale);
    }

    private void prjOpacityToZero()
    {
        NoteAnimation.SetOpacityToZero(prjSprite);
    }

    private void prjDecreaseOpacity()
    {
        NoteAnimation.ChangeOpacity(
            sprite: prjSprite,
            amount: NoteAnimation.prjDisappearAmount
        );
    }

    private void prjIncreaseOpacity()
    {
        NoteAnimation.ChangeOpacity(
            sprite: prjSprite,
            amount: NoteAnimation.prjAppearAmount
        );
    }

    private void prjDecreaseScaleFast()
    {
        NoteAnimation.ChangeScale(
            transform: prjTransform, 
            amount: NoteAnimation.prjScaleDownFastAmount
        ); 
    }

    private void prjDecreaseScaleSlow()
    {
        NoteAnimation.ChangeScale(
            transform: prjTransform, 
            amount: NoteAnimation.prjScaleDownSlowAmount
        ); 
    }

    private void prjIncreaseScale()
    {
        NoteAnimation.ChangeScale(
            transform: prjTransform, 
            amount: NoteAnimation.prjScaleUpAmount
        ); 
    }

    private bool isRingMaxScale()
    {
        return NoteAnimation.IsMaxScale(ringTransform, NoteAnimation.ringMaxScale);
    }
    
    private void ringOpacityToZero()
    {
        NoteAnimation.SetOpacityToZero(ringSprite);
    }

    private void ringDecreaseOpacity()
    {
        NoteAnimation.ChangeOpacity(
            sprite: ringSprite,
            amount: NoteAnimation.ringDisappearAmount
        );
    }

    private void ringIncreaseOpacity()
    {
        NoteAnimation.ChangeOpacity(
            sprite: ringSprite,
            amount: NoteAnimation.ringAppearAmount
        );
    }

    private void ringIncreaseScale()
    {
        NoteAnimation.ChangeScale(
            transform: ringTransform, 
            amount: NoteAnimation.ringScaleUpAmount
        ); 
    }

    private void linkOpacityToZero()
    {
        NoteAnimation.SetLineOpacityToZero(linkRenderer);
    }

    private void linkIncreaseOpacity()
    {
        NoteAnimation.ChangeLineOpacity(
            line: linkRenderer,
            amount: NoteAnimation.linkAppearAmount
        );
    }

}