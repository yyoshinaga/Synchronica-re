// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;


// public class GreenTargetScript : MonoBehaviour
// {
//     //Score
//     private GameObject player;
//     private int touchScore;

//     //Touch stuff
//     public List<TouchIndicator> touches = new List<TouchIndicator>();
//     private bool targetIsReady;
//     private bool hasBeenHit;
//     private bool wordAppeared;
//     private bool miss;
//     private bool earlyTouch;
//     private bool perfectTouch;
//     private bool lateTouch;
//     private bool tooLateTouch;
//     private bool doneOnce;
//     private int touchID;

//     //Link
//     public GameObject link;

//     //Target 
//     private Transform Target;
//     private Vector2 targetScale;
//     private SpriteRenderer targetSprite;
//     private Color targetColor;
//     // public TargetType targetType;
//     private bool moveNoteToPerfection;

//     //Image
//     public Transform GreenImage;

//     //Note
//     private Transform Note;
//     private Transform myTransform;
//     private Vector2 noteScale;
//     private Color noteColor;
//     private SpriteRenderer noteSprite;

//     //Ring
//     private Transform Ring;
//     private Vector2 ringScale;
//     private Color ringColor;
//     private SpriteRenderer ringSprite;

//     //Green only
//     public float desiredTouchDirection;
//     private Vector2 startPos;

//     //Variables
//     private float timeVariable;
//     private float visibleVariable;
//     private float flipVariable;
//     private float noteScaleSteps;
//     private float noteDecrement;
//     private float firingAngle;
//     private float gravity;
//     private float ringScaleSteps;

//     //Text objects
//     private GameObject earlyText;
//     private GameObject greatText;
//     private GameObject perfectText;
//     private GameObject slowText;
//     private GameObject missedText;

//     //Audio stuff
//     private AudioSource audioSource;
//     private bool hasNotPlayedSound;
//     public bool hitSoundEnabled;

//     void Start()
//     {

//         Input.multiTouchEnabled = true;

//         //Score
//         player = GameObject.FindWithTag("Player");
//         touchScore = 0;

//         //Touch stuff
//         targetIsReady = false;
//         miss = false;
//         wordAppeared = false;
//         earlyTouch = false;
//         perfectTouch = false;
//         lateTouch = false;
//         tooLateTouch = false;
//         doneOnce = false;

//         //Target
//         Target = transform;
//         targetScale = Target.localScale;
//         targetScale.x = 0.02f;  //Divisible by 0.2
//         targetScale.y = 0.02f;
//         transform.localScale = targetScale;
//         moveNoteToPerfection = false;

//         //Image
//         GreenImage = gameObject.transform.GetChild(0);
//         targetSprite = GreenImage.GetComponent<SpriteRenderer>();
//         targetColor = targetSprite.color;
//         targetColor.a = 0.0f;                 //Increase the opacity
//         targetSprite.color = targetColor;

//         //Note 
//         Note = gameObject.transform.GetChild(1);
//         noteScale = Note.localScale;
//         noteSprite = Note.GetComponent<SpriteRenderer>();
//         noteColor = noteSprite.color;
//         noteColor.a = 0.0f;                 //Increase the opacity
//         noteSprite.color = noteColor;

//         //Ring 
//         Ring = gameObject.transform.GetChild(2);
//         ringScale = Ring.localScale;
//         ringSprite = Ring.GetComponent<SpriteRenderer>();
//         ringColor = ringSprite.color;
//         ringColor.a = 0.0f;                 //Increase the opacity
//         ringSprite.color = ringColor;

//         //Variables
//         visibleVariable = 50;
//         flipVariable = -60;
//         noteScaleSteps = 0.025f;
//         noteDecrement = 0.025f;
//         firingAngle = 80f;
//         gravity = 200f;
//         ringScaleSteps = 0.025f;

//         //Text stuff
//         earlyText = gameObject.transform.GetChild(3).gameObject;
//         greatText = gameObject.transform.GetChild(4).gameObject;
//         perfectText = gameObject.transform.GetChild(5).gameObject;
//         slowText = gameObject.transform.GetChild(6).gameObject;
//         missedText = gameObject.transform.GetChild(7).gameObject;

//         earlyText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         greatText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         perfectText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         slowText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         missedText.GetComponentInChildren<TMP_Text>().alpha = 0;

//         //Audio stuff
//         audioSource = transform.GetComponent<AudioSource>();
//         hasNotPlayedSound = true;

//         StartCoroutine(SimulateProjectile());

//     }


//     void Awake()
//     {
//         myTransform = transform.GetChild(1);
//     }

//     IEnumerator SimulateProjectile()
//     {
//         while (targetScale.x < 0.5f && targetScale.y < 0.5f)
//         {
//             GrowTarget();
//             Note.position = Target.position;
//             yield return null;
//         }
//         targetIsReady = true;
//         // Short delay added before Projectile is thrown
//         //yield return new WaitForSeconds(1.5f);

//         // Move projectile to the position of throwing object + add some offset if needed.
//         Note.position = myTransform.position + new Vector3(8f, -5f, 0);

//         // Calculate distance to target
//         float targetDistance = transform.InverseTransformPoint(Note.position).magnitude - 10;             

//         // Calculate the velocity needed to throw the object to the target at specified angle.
//         float projectileVelocity = Mathf.Sqrt(targetDistance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity));

//         // Extract the X  Y componenent of the velocity
//         float Vx = projectileVelocity * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
//         float Vy = projectileVelocity * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

//         // Calculate flight time.
//         float flightDuration = targetDistance / Vx;

//         // Rotate projectile to face the target.
//         //Projectile.rotation = Quaternion.LookRotation(Target.position - Projectile.position);

//         float elapse_time = 0;

//         float thresholdTime = 0.05f;
//         float comparisonDuration = flightDuration + thresholdTime + 0.3f;
//         bool keepShrinking = false;

//         //The 0.3 is for how many seconds after throwing the note to destroy it
//         while (elapse_time < comparisonDuration)
//         {
//             if (elapse_time < flightDuration + thresholdTime && !hasBeenHit)
//             {
//                 timeVariable = Vy - (gravity * elapse_time);                    //Goes from positive 103 to -111?

//                 if (timeVariable >= visibleVariable)                           //Going up, note remains invisible
//                 {
//                     GrowNoteInvisible();
//                 }
//                 else if (timeVariable < visibleVariable && timeVariable > 0f)   //Going up, note visible
//                 {
//                     GrowNote();
//                 }
//                 else if (timeVariable > flipVariable)
//                 {
//                     ShrinkNoteSlow();
//                     if (timeVariable < flipVariable + 45)
//                     {
//                         earlyTouch = true;
//                     }
//                     if (timeVariable < flipVariable + 27)
//                     {
//                         perfectTouch = true;
//                     }
//                     if (timeVariable < flipVariable + 10)
//                     {
//                         moveNoteToPerfection = true;
//                     }
//                 }
//                 else                                                            //Going down, rapidly
//                 {
//                     if (timeVariable > flipVariable - 10)
//                     {
//                         moveNoteToPerfection = true;
//                     }
//                     if (timeVariable > flipVariable - 27)
//                     {
//                         perfectTouch = true;
//                     }
//                     else
//                     {
//                         lateTouch = true;
//                     }
//                     keepShrinking = true;
//                     noteSprite.sortingOrder = -1;
//                 }

//                 Note.Translate(-Vx * Time.deltaTime, (Vy - (gravity * elapse_time)) * Time.deltaTime, 0);
//             }
//             else if(hasBeenHit)
//             {
//                 break;
//             }
//             if (keepShrinking)
//             {
//                 ShrinkNoteRapid();
//             }
//             elapse_time += Time.deltaTime;
//             yield return null;
//         }
        
//         //This destructionPhase must happen without any if statements
//         DestructionPhase();     //Gets destroyed 
//     }


//     // Update is called once per frame
//     void Update()
//     {
//         for (int i = 0; i < Input.touchCount; i++)
//         {
//             Touch touch = Input.GetTouch(i);
//             Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
//             RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

//             if (hit.collider != null && targetIsReady && !miss)
//             {
//                 Debug.Log(desiredTouchDirection);

//                 if (hit.collider.transform.GetInstanceID() == gameObject.transform.GetInstanceID())
//                 {

//                     if (touch.phase == TouchPhase.Began)
//                     {
                    
//                         Destroy(link);
//                         MakeNoteMoveOrDisappear(moveNoteToPerfection);
//                         startPos = touch.position;
//                         touchID = touch.fingerId;

//                     }
               

//                     if (touchID == touch.fingerId && touch.phase == TouchPhase.Moved)
//                     {

//                         //I know, it's supposed to be tanInv(y,x), but its good for this game
//                         float measuredTouchDirection = Mathf.Atan2((touch.position.x - startPos.x), (touch.position.y - startPos.y)) * Mathf.Rad2Deg;

//                         if (measuredTouchDirection < -180)
//                         {
//                             Debug.Log("Added 360");
//                             measuredTouchDirection += 360;
//                         }
//                         if(desiredTouchDirection > 180)
//                         {
//                             Debug.Log("going to subtract");
//                             Debug.Log(desiredTouchDirection);
//                             desiredTouchDirection -= 360;
//                         }


//                         //Has to flick with greater magnitude of 2 and within +- 20 deg 
//                         if ((touch.position - startPos).magnitude > 2 && measuredTouchDirection < desiredTouchDirection + 20 && measuredTouchDirection > desiredTouchDirection - 20)
//                         {
//                             //Don't erase this. It's needed
//                             if (hit.collider.transform.GetInstanceID() == gameObject.transform.GetInstanceID() && !wordAppeared)  //Check to see if touch is colliding with target and Rate the score
//                             {
          
//                                 if (hitSoundEnabled && hasNotPlayedSound)
//                                 {
//                                     audioSource.PlayOneShot(audioSource.clip, 1f);
//                                     hasNotPlayedSound = false;
//                                 }

//                                 wordAppeared = true;    //Prevent from happening twice
//                                 hasBeenHit = true;      //Start bursting the ring
//                                 SlightlyScaleDownTarget();                                      //When note is touched, the target gets scaled down a bit


//                                 if (!earlyTouch)
//                                 {
//                                     //Debug.Log("Early");
//                                     earlyText.GetComponentInChildren<Transform>().position = transform.position;
//                                     earlyText.GetComponentInChildren<TMP_Text>().alpha = 1;
//                                     player.GetComponent<ScoreManager>().AddScore(0, 4);
//                                 }
//                                 else if (earlyTouch && !perfectTouch)
//                                 {
//                                     //Debug.Log("Great");
//                                     greatText.GetComponentInChildren<Transform>().position = transform.position;
//                                     greatText.GetComponentInChildren<TMP_Text>().alpha = 1;
//                                     touchScore = 5;
//                                     player.GetComponent<ScoreManager>().AddScore(touchScore, 2);
//                                 }
//                                 else if (perfectTouch && !lateTouch)
//                                 {
//                                     //Debug.Log("Perfect");
//                                     perfectText.GetComponentInChildren<Transform>().position = transform.position;
//                                     perfectText.GetComponentInChildren<TMP_Text>().alpha = 1;
//                                     touchScore = 10;
//                                     player.GetComponent<ScoreManager>().AddScore(touchScore, 1);
//                                 }
//                                 else if (lateTouch)
//                                 {
//                                     Debug.Log("Slow");
//                                     slowText.GetComponentInChildren<Transform>().position = transform.position;
//                                     slowText.GetComponentInChildren<TMP_Text>().alpha = 1;
//                                     touchScore = 1;
//                                     player.GetComponent<ScoreManager>().AddScore(touchScore, 3);
//                                 }
//                                 else
//                                 {
//                                     Debug.Log("SOMETHING WENT WRONG ");
//                                 }
//                             }
//                         }
//                         else
//                         {
//                             Debug.Log(measuredTouchDirection);
//                             Debug.Log(desiredTouchDirection);
//                             Debug.Log("----------------------------------------");
//                         }
//                     }
//                 }
//             }
//         }

//         //If no words appeared and its too late to touch,
//         if (!wordAppeared && tooLateTouch)
//         {
//             missedText.GetComponentInChildren<Transform>().position = transform.position;
//             missedText.GetComponentInChildren<TMP_Text>().alpha = 1;
//             player.GetComponent<ScoreManager>().AddScore(0,5);
//             wordAppeared = true;
//             miss = true;
//         }
//     }

//     void MakeNoteMoveOrDisappear(bool isPerfect)
//     {
//         if (isPerfect)
//         {
//             Note.localPosition = new Vector2(0, -4);
//         }
//         else
//         {
//             noteColor.a = 0f;                 //Make note disappear
//             noteSprite.color = noteColor;
//         }
//     }

//     void DestructionPhase()
//     {
//         //Turn off collider to not disturb other notes underneath
//         gameObject.transform.GetComponent<CircleCollider2D>().enabled = false;
//         tooLateTouch = true;
//         if (!hasBeenHit)
//         {
//             noteColor.a = 0.0f;                 //Increase the opacity
//             noteSprite.color = noteColor;
//             Destroy(link);

//         }
//         else
//         {
//             if (touchScore != 0)
//             {
//                 switch (touchScore)
//                 {
//                     case 5:
//                         ringColor = Color.green;
//                         break;
//                     case 10:
//                         ringColor = Color.white;
//                         break;
//                     case 1:
//                         ringColor = Color.grey;
//                         break;
//                     case 0:
//                         ringColor = Color.grey;
//                         break;
//                 }
//             }
//         }
//         StartCoroutine(DisappearingSequence());
//     }

//     void GrowTarget()
//     {
//         targetScale.x += 0.02f;
//         targetScale.y += 0.02f;
//         transform.localScale = targetScale;
//         targetColor.a += 0.1f;                 //Increase the opacity
//         targetSprite.color = targetColor;
//     }
//     void GrowNoteInvisible()
//     {
//         noteScale.x += noteScaleSteps;
//         noteScale.y += noteScaleSteps;
//         Note.localScale = noteScale;
//     }
//     void GrowNote()
//     {
//         noteScale.x += noteScaleSteps;
//         noteScale.y += noteScaleSteps;
//         Note.localScale = noteScale;
//         noteColor.a += 0.1f;                 //Increase the opacity
//         noteSprite.color = noteColor;
//     }
//     void ShrinkNoteSlow()
//     {
//         noteScale.x -= noteDecrement;
//         noteScale.y -= noteDecrement;
//         Note.localScale = noteScale;
//     }
//     void ShrinkNoteRapid()
//     {
//         if (noteScale.x > 0.01f)
//         {
//             noteScale.x -= 0.05f;
//             noteScale.y -= 0.05f;
//             Note.localScale = noteScale;
//         }
//         else
//         {
//             noteColor.a = 0.0f;                 //Make it disappear
//             noteSprite.color = noteColor;
//         }
//     }
//     void SlightlyScaleDownTarget()
//     {
//         targetScale.x -= 0.05f;
//         targetScale.y -= 0.05f;
//         Target.localScale = targetScale;
//     }
//     IEnumerator DisappearingSequence()
//     {
//         //Destruction phase
//         while (targetScale.x < 0.53)
//         {
//             targetColor.a -= 0.1f;                 //Increase the opacity
//             targetSprite.color = targetColor;
//             noteColor.a -= 0.1f;                 //Increase the opacity
//             noteSprite.color = noteColor;

//             if (hasBeenHit)
//             {
//                 targetScale.x += 0.001f;
//                 targetScale.y += 0.001f;
//                 transform.localScale = targetScale;

//                 ringScale.x += ringScaleSteps;
//                 ringScale.y += ringScaleSteps;
//                 Ring.localScale = ringScale;

//                 if (!doneOnce && ringColor.a < 1)
//                 {
//                     ringColor.a += 0.2f;                 //Increase the opacity
//                     ringSprite.color = ringColor;
//                     if (ringColor.a >= 1)
//                     {
//                         ringColor.a = 1;
//                         doneOnce = true;
//                     }
//                 }
//                 else
//                 {
//                     if (ringScale.x < 2.25 && ringColor.a > 0)
//                     {
//                         ringColor.a -= 0.015f;                 //Decrease the opacity
//                         ringSprite.color = ringColor;
//                     }
//                     else
//                     {
//                         ringColor.a = 0f;                 //Decrease the opacity
//                         ringSprite.color = ringColor;
//                     }
//                 }
//                 if (targetScale.x >= 0.53)
//                 {
//                     Destroy(gameObject);
//                 }
//             }
//             else
//             {
//                 if (targetColor.a < 0)
//                 {
//                     Destroy(gameObject);
//                 }
//             }

//             yield return null;
//         }

//     }
// }
