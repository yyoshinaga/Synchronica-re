// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;


// public class RedTargetScript : MonoBehaviour
// {
//     //Score
//     private GameObject player;
//     private int touchScore;

//     //Touch stuff
//     public List<TouchIndicator> touches = new List<TouchIndicator>();
//     private int t; //Touches
//     private bool targetIsReady;
//     private bool hasBeenHit;
//     private bool wordAppeared;
//     private bool miss;
//     private bool earlyTouch;
//     private bool perfectTouch;
//     private bool lateTouch;
//     private bool tooLateTouch;
//     private bool doneOnce;
//     private float touchedLength;
//     private bool beingTouched;
//     private int touchID;          ///FINGERRRRRRRRRRRRRRRRRRRRRRRRRRRRR ID

//     //Link
//     public GameObject link;

//     //Target 
//     private Transform Target;
//     private bool targetFullyGrown;
//     private Vector2 targetScale;
//     private SpriteRenderer targetSprite;
//     private Color targetColor;
//     public float holdLength;
//     // public TargetType targetType;
//     private bool moveNoteToPerfection;

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

//     //Loader
//     private GameObject loader;
//     private Image rightLoader;
//     private Image leftLoader;
//     private float currentValue;
//     private float speed;
//     private bool explodeLoader;
//     private bool released;
//     private float startTime;

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
//     private GameObject bonusText;


//     //Audio stuff
//     public AudioSource audioSource;
//     public AudioSource endAudioSource;
//     private bool hasNotPlayedSound;
//     private bool hasNotPlayedEndSound;
//     public bool hitSoundEnabled;


//     void Start()
//     {

//         Input.multiTouchEnabled = true;
//         //Score
//         player = GameObject.FindWithTag("Player");
//         touchScore = 0;

//         //Touch stuff
//         targetIsReady = false;
//         miss = true;
//         wordAppeared = false;
//         earlyTouch = false;
//         perfectTouch = false;
//         lateTouch = false;
//         tooLateTouch = false;
//         doneOnce = false;
//         beingTouched = false;

//         //Target
//         Target = transform;
//         targetScale = Target.localScale;
//         targetScale.x = 0.02f;  //Divisible by 0.2
//         targetScale.y = 0.02f;
//         transform.localScale = targetScale;
//         targetSprite = Target.GetComponent<SpriteRenderer>();
//         targetColor = targetSprite.color;
//         targetColor.a = 0.0f;                 //Increase the opacity
//         targetSprite.color = targetColor;
//         moveNoteToPerfection = false;

//         //Note 
//         Note = gameObject.transform.GetChild(0);
//         noteScale = Note.localScale;
//         noteSprite = Note.GetComponent<SpriteRenderer>();
//         noteColor = noteSprite.color;
//         noteColor.a = 0.0f;                 //Increase the opacity
//         noteSprite.color = noteColor;

//         //Ring 
//         Ring = gameObject.transform.GetChild(1);
//         ringScale = Ring.localScale;
//         ringSprite = Ring.GetComponent<SpriteRenderer>();
//         ringColor = ringSprite.color;
//         ringColor.a = 0.0f;                 //Increase the opacity
//         ringSprite.color = ringColor;

//         //Variables
//         visibleVariable = 50;
//         flipVariable = -55;
//         noteScaleSteps = 0.025f;
//         noteDecrement = 0.025f;
//         firingAngle = 80f;
//         gravity = 200f;
//         ringScaleSteps = 0.025f;

//         //Text stuff
//         earlyText = gameObject.transform.GetChild(2).gameObject;
//         greatText = gameObject.transform.GetChild(3).gameObject;
//         perfectText = gameObject.transform.GetChild(4).gameObject;
//         slowText = gameObject.transform.GetChild(5).gameObject;
//         missedText = gameObject.transform.GetChild(6).gameObject;

//         earlyText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         greatText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         perfectText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         slowText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         missedText.GetComponentInChildren<TMP_Text>().alpha = 0;


//         //Only for red
//         bonusText = gameObject.transform.GetChild(7).gameObject;
//         bonusText.GetComponentInChildren<TMP_Text>().alpha = 0;

//         loader = gameObject.transform.GetChild(8).gameObject;
//         rightLoader = loader.GetComponentsInChildren<Image>()[0];
//         leftLoader = loader.GetComponentsInChildren<Image>()[1];
//         rightLoader.fillAmount = 0;
//         leftLoader.fillAmount = 0;
//         currentValue = 0;
//         //holdLength = 2.0f;
//         speed = 0.5f / holdLength;
//         explodeLoader = false;
//         released = false;


//         //Audio stuff
//         hasNotPlayedSound = true;
//         hasNotPlayedEndSound = true;
//         StartCoroutine(SimulateProjectile());

//     }





//     void Awake()
//     {
//         myTransform = transform.GetChild(0);
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
//                 //Debug.Log(timeVariable);
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
//                     if (timeVariable < flipVariable + 37)
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
//             else if (hasBeenHit)
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
//         if (!hasBeenHit)
//         {
//             DestructionPhase();     //Gets destroyed 
//         }
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (!tooLateTouch)
//         {
//             for (int i = 0; i < Input.touchCount; i++)
//             {
//                 Touch touch = Input.GetTouch(i);

//                 Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
//                 RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

//                 if (hit.collider != null && targetIsReady && touch.phase == TouchPhase.Began)
//                 {

//                     if (hit.collider.transform.GetInstanceID() == gameObject.transform.GetInstanceID())  //Check to see if touch is colliding with target
//                     {
//                         Destroy(link);
//                         MakeNoteMoveOrDisappear(moveNoteToPerfection);


//                         //Save the touchID for later
//                         touchID = touch.fingerId;
//                         miss = false;

//                         //Rate the score
//                         if (!wordAppeared)
//                         {

//                             if (hitSoundEnabled && hasNotPlayedSound)
//                             {
//                                 audioSource.Play(); // (audioSource.clip, 1f);
//                                 hasNotPlayedSound = false;
//                             }

//                             wordAppeared = true;    //Prevent from happening twice
//                             hasBeenHit = true;      //Start bursting the ring
//                             SlightlyScaleDownTarget();                                      //When note is touched, the target gets scaled down a bit

//                             if (!earlyTouch)
//                             {
//                                 //Debug.Log("Early");
//                                 earlyText.GetComponentInChildren<Transform>().position = transform.position;
//                                 earlyText.GetComponentInChildren<TMP_Text>().alpha = 1;
//                                 touchScore = 0;
//                                 player.GetComponent<ScoreManager>().AddScore(touchScore, 4);

//                             }
//                             else if (earlyTouch && !perfectTouch)
//                             {
//                                 //Debug.Log("Great");
//                                 greatText.GetComponentInChildren<Transform>().position = transform.position;
//                                 greatText.GetComponentInChildren<TMP_Text>().alpha = 1;
//                                 touchScore = 5;
//                                 player.GetComponent<ScoreManager>().AddScore(touchScore, 2);
//                             }
//                             else if (perfectTouch && !lateTouch)
//                             {
//                                 //Debug.Log("Perfect");
//                                 perfectText.GetComponentInChildren<Transform>().position = transform.position;
//                                 perfectText.GetComponentInChildren<TMP_Text>().alpha = 1;
//                                 touchScore = 10;
//                                 player.GetComponent<ScoreManager>().AddScore(touchScore, 1);
//                             }
//                             else if (lateTouch)
//                             {
//                                 //Debug.Log("Slow");
//                                 slowText.GetComponentInChildren<Transform>().position = transform.position;
//                                 slowText.GetComponentInChildren<TMP_Text>().alpha = 1;
//                                 touchScore = 1;
//                                 player.GetComponent<ScoreManager>().AddScore(touchScore, 3);
//                             }
//                             else
//                             {
//                                 Debug.Log("SOMETHING WENT WRONG ");
//                             }
//                         }
//                     }
//                 }


//                 if (!beingTouched && hasBeenHit && touch.phase != TouchPhase.Ended && !miss)
//                 {
//                     beingTouched = true;
//                     startTime = Time.time;
//                     StartCoroutine(LoadRedCircle());
//                 }

//                 else if (hasBeenHit && touch.fingerId == touchID && (touch.phase == TouchPhase.Ended || explodeLoader))
//                 {
//                     //Stop Audio
//                     audioSource.Stop();

//                     //When touch ends
//                     released = true;

//                     //Stop the red circle loader and make it disappear
//                     loader.GetComponentInChildren<CanvasGroup>().alpha = 0;
//                     bonusText.GetComponentInChildren<Transform>().position = transform.position + new Vector3(0, 5f, 0);


//                     //Give score
//                     if (rightLoader.fillAmount >= 0.35 && rightLoader.fillAmount <= 0.4)
//                     {
//                         bonusText.GetComponentInChildren<TMP_Text>().color = new Color32(180, 0, 255, 255);
//                         bonusText.GetComponentInChildren<TMP_Text>().alpha = 1;

//                         player.GetComponent<ScoreManager>().AddScore(2, 0);  //Essentially, adding 3 more points
//                     }
//                     else if (rightLoader.fillAmount >= 0.4 && rightLoader.fillAmount <= 0.55)
//                     {
//                         bonusText.GetComponentInChildren<TMP_Text>().color = new Color32(255, 255, 255, 255);
//                         bonusText.GetComponentInChildren<TMP_Text>().alpha = 1;

//                         player.GetComponent<ScoreManager>().AddScore(10, 0);  //Essentially, adding 7 more points
//                     }
//                     else if (explodeLoader)     //User went past the explosion point
//                     {
//                         bonusText.GetComponentInChildren<TMP_Text>().color = new Color32(180, 0, 255, 255);
//                         bonusText.GetComponentInChildren<TMP_Text>().alpha = 1;

//                         player.GetComponent<ScoreManager>().AddScore(2, 0);  //Essentially, adding 3 more points 
//                     }




//                     //Once the touch ends for red, the game object goes into the destruction phase
//                     //Debug.Log("END TOUCH");
//                     DestructionPhase();
//                 }

//                 if (hasBeenHit && touch.phase != TouchPhase.Ended && explodeLoader) //User keeps finger on past loader
//                 {
//                     DestructionPhase();
//                 }
//             }
//         }


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
//             Note.localPosition = Vector2.zero;
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
//             if (hitSoundEnabled && hasNotPlayedEndSound)
//             {
//                 if (audioSource.isPlaying)
//                 {
//                     audioSource.Stop(); // (audioSource.clip, 1f);
//                 }
//                 endAudioSource.Play(); // (audioSource.clip, 1f);
//                 hasNotPlayedEndSound = false;
//             }
//             if (touchScore != 0)
//             {
//                 switch (touchScore)
//                 {
//                     case 3:
//                         ringColor = Color.red;
//                         break;
//                     case 7:
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

//     IEnumerator LoadRedCircle()
//     {

//         while (rightLoader.fillAmount <= 0.6 && !released && !explodeLoader) //Not 0.5 since the user has to stop at 0.5
//         {
//             currentValue = speed * (Time.time-startTime);
//             rightLoader.fillAmount += currentValue;
//             leftLoader.fillAmount += currentValue ;    
//             startTime = Time.time;
//             yield return null;
//         }
//         explodeLoader = true;

//     }

//     int VibrateTarget(int counter)
//     {
//         Debug.Log(counter);
//         counter += 10;
//         targetScale.x += Mathf.Sin(counter * 180 / Mathf.PI);
//         targetScale.y += Mathf.Sin(counter * 180 / Mathf.PI);
//         transform.localScale = targetScale;
//         return counter;
//     }
//     void GrowTarget()
//     {
//         targetScale.x += 0.02f;
//         targetScale.y += 0.02f;
//         transform.localScale = targetScale;
//         targetColor.a = targetColor.a + 0.1f;                 //Increase the opacity
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
//         noteColor.a = noteColor.a + 0.1f;                 //Increase the opacity
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
//         //noteScale.x += 0.05f;
//         //noteScale.y += 0.05f;

//         Target.localScale = targetScale;
//         //Note.localScale = noteScale;
//     }
//     IEnumerator DisappearingSequence()
//     {
//         //Destruction phase
//         while (targetScale.x < 0.53)
//         {


//             targetColor.a = targetColor.a - 0.1f;                 //Increase the opacity
//             targetSprite.color = targetColor;
//             noteColor.a = noteColor.a - 0.1f;                 //Increase the opacity
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
//                     EverythingDisappear();

//                     Destroy(gameObject, endAudioSource.clip.length);
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

//     //Since it will take a while for the audio to end, make everything disappear in the mean time
//     void EverythingDisappear()
//     {
//         loader.GetComponentInChildren<CanvasGroup>().alpha = 0;
//         earlyText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         earlyText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         greatText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         perfectText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         slowText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         missedText.GetComponentInChildren<TMP_Text>().alpha = 0;
//         bonusText.GetComponentInChildren<TMP_Text>().alpha = 0;

//     }
// }

