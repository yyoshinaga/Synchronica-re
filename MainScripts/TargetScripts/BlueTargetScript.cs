// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;


// public class BlueTarget : GenericNote
// {
//     private GameObject player;
//     private int touchScore;

//     //Touch stuff
//     public List<TouchIndicator> touches = new List<TouchIndicator>();
//     // private bool targetIsReady;
//     private bool hasBeenHit;
//     private bool wordAppeared;
//     private bool miss;
//     // private bool earlyTouch;
//     // private bool perfectTouch;
//     // private bool lateTouch;
//     // private bool tooLateTouch;
//     private bool doneOnce;
//     private TouchTypes touchEval;

//     //Link
//     public GameObject link;

//     //Target 
//     private Transform Target;
//     private Vector2 targetScale;
//     private SpriteRenderer targetSprite;
//     private Color targetColor;
//     // public TargetType targetType;
//     private bool moveNoteToPerfection;
//     private ProjectileVariables pv;

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

//     //Variables
//     private float currentVelocity;
//     private float visibleVariable;
//     private float ringScaleSteps;
//     private float maxTargetSize;

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
//         //Score
//         player = GameObject.FindWithTag("Player");
//         touchScore = 0;

//         //Touch stuff
//         miss = false;
//         wordAppeared = false;
//         doneOnce = false;
//         touchEval = TouchTypes.TOOEARLY;

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
//         maxTargetSize = 0.5f;

//         pv = new ProjectileVariables(); // Initialize first
//         pv.targetIsReady = false;   // Only need this for initialization

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

//         //Audio stuff
//         audioSource = transform.GetComponent<AudioSource>();
//         hasNotPlayedSound = true;

//         StartCoroutine(SimulateProjectile());
//     }

//     void Awake()
//     {
//         myTransform = transform.GetChild(0);
//     }

//     IEnumerator SimulateProjectile()
//     {
//         while (transform.localScale.x < maxTargetSize && transform.localScale.y < maxTargetSize)
//         {
//             ScaleAndColor scaleAndColor = GrowTarget(transform.localScale, targetSprite.color);
//             transform.localScale = scaleAndColor.scale;
//             targetSprite.color = scaleAndColor.color;

//             Note.position = Target.position;
//             yield return null;
//         }

//         // Move projectile to the position of throwing object + add some offset if needed.
//         Note.position = myTransform.position + new Vector3(8f, -5f, 0);

//         // update local projectile variables
//         pv = ProjectileParameters(Note.position);

//         //The 0.3 is for how many seconds after throwing the note to destroy it
//         while (pv.elapseTime < pv.comparisonDuration)
//         {
//             if (pv.elapseTime < pv.flightDuration + pv.thresholdTime && !hasBeenHit)
//             {
//                 // Simulate current velocity with elapsedTime
//                  //Goes from positive 103 to -111?
//                 currentVelocity = pv.velY - (pv.gravity * pv.elapseTime);                   
//                 //Going up, note remains invisible
//                 if (currentVelocity >= visibleVariable)                            
//                 {
//                     Note.localScale = GrowNoteInvisible(Note.localScale);
//                 }
//                 //Going up, note visible
//                 else if (currentVelocity < visibleVariable && currentVelocity > 0f)   
//                 {
//                     ScaleAndColor scaleAndColor = GrowNote(Note.localScale, noteSprite.color);
//                     Note.localScale = scaleAndColor.scale;
//                     noteSprite.color = scaleAndColor.color;
//                 }
//                 else
//                 {
//                     touchEval = EvaluateTouch(currentVelocity);
//                     print(touchEval + "evaluated touch");
        
//                     if (currentVelocity < flipVariable + 10 && 
//                         currentVelocity > flipVariable - 10)
//                     {
//                         moveNoteToPerfection = true;
//                     }

//                     if (currentVelocity > flipVariable)
//                     {
//                         Note.localScale = ShrinkNoteSlow(Note.localScale);
//                     }
//                     else
//                     {
//                         pv.keepShrinking = true;
//                         noteSprite.sortingOrder = -1;
//                     }
//                 }
             
//                 Note.Translate(-pv.velX * Time.deltaTime, 
//                     (pv.velY - (pv.gravity * pv.elapseTime)) * Time.deltaTime, 0
//                 );
//             }
//             else if (hasBeenHit)
//             {
//                 break;
//             }

//             if (pv.keepShrinking)
//             {
//                 // If less than 0.01, make it transparent
//                 ScaleAndColor scaleAndColor = 
//                     ShrinkNoteRapid(Note.localScale, noteSprite.color);
//                 Note.localScale = scaleAndColor.scale;
//                 noteSprite.color = scaleAndColor.color;
//             }
               
//             pv.elapseTime += Time.deltaTime;
//             yield return null;
//         }

//         //This destructionPhase must happen without any if statements
//         touchEval = TouchTypes.TOOLATE;
//         DestructionPhase();     //Gets destroyed if you don't press 
//     }

//     // Update is called once per frame
//     void Update()
//     {
// 		if (Input.GetMouseButtonDown(0))
//         {
// 			// for (int i = 0; i < Input.touchCount; i++)
// 			// {
// 				//! Get rid of the if statement outside of the fore loop
// 				Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
// 				// Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.touches[i].position);
// 				RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

// 				if (hit.collider != null && pv.targetIsReady && !miss)
// 				{
// 					if (hit.collider.transform.GetInstanceID() == 
// 						gameObject.transform.GetInstanceID())  //Check to see if touch is colliding with target
// 					{
// 						if (hitSoundEnabled && hasNotPlayedSound)
// 						{
// 							audioSource.PlayOneShot(audioSource.clip, 1f);
// 							hasNotPlayedSound = false;
// 						}
// 						Destroy(link);

// 						ScaleAndColor scaleAndColor = 
// 							MakeNoteMoveOrDisappear(moveNoteToPerfection, noteScale, noteColor);
// 						Note.localPosition = scaleAndColor.scale;
// 						noteSprite.color = scaleAndColor.color;

// 						//Rate the score
// 						if (!wordAppeared)
// 						{
// 							wordAppeared = true;    //Prevent from happening twice
// 							hasBeenHit = true;      //Start bursting the ring

// 							//When note is touched, the target gets scaled down a bit
// 							Target.localScale = SlightlyScaleDownTarget(Target.localScale); 

// 							switch (touchEval)
// 							{
// 								case TouchTypes.TOOEARLY:
// 									earlyText.GetComponentInChildren<Transform>().position = 
// 										transform.position;
// 									earlyText.GetComponentInChildren<TMP_Text>().alpha = 1;
// 									touchScore = 0;
// 									player.GetComponent<ScoreManager>().AddScore(touchScore, 4);
// 									break;
// 								case TouchTypes.EARLY:
// 									greatText.GetComponentInChildren<Transform>().position = 
// 										transform.position;
// 									greatText.GetComponentInChildren<TMP_Text>().alpha = 1;
// 									touchScore = 5;
// 									player.GetComponent<ScoreManager>().AddScore(touchScore,2);
// 									break;
// 								case TouchTypes.PERFECT:
// 									perfectText.GetComponentInChildren<Transform>().position = 
// 										transform.position;
// 									perfectText.GetComponentInChildren<TMP_Text>().alpha = 1;
// 									touchScore = 10;
// 									player.GetComponent<ScoreManager>().AddScore(touchScore,1);
// 									break;
// 								case TouchTypes.LATE:
// 									slowText.GetComponentInChildren<Transform>().position = 
// 										transform.position;
// 									slowText.GetComponentInChildren<TMP_Text>().alpha = 1;
// 									touchScore = 1;
// 									player.GetComponent<ScoreManager>().AddScore(touchScore,3);
// 									break;
// 								default:
// 									Debug.Log("SOMETHING WENT WRONG ");
// 									break;
// 							}
// 						}
// 					}
// 				}
// 			}
//         // }
//         if (!wordAppeared && touchEval == TouchTypes.TOOLATE)
//         {
//             print("you misssed");
//             missedText.GetComponentInChildren<Transform>().position = transform.position;
//             missedText.GetComponentInChildren<TMP_Text>().alpha = 1;
//             player.GetComponent<ScoreManager>().AddScore(0, 5);
//             wordAppeared = true;
//             miss = true;
//         }
//     }

//     void DestructionPhase()
//     {
//         //Turn off collider to not disturb other notes underneath
//         gameObject.transform.GetComponent<CircleCollider2D>().enabled = false;
//         if (!hasBeenHit)
//         {
//             noteColor.a = 0.0f;                 //Increase the opacity
//             noteSprite.color = noteColor;
//             Destroy(link);
//         }
//         else
//         {
//             switch (touchScore)
//             {
//                 case 5:
//                     ringColor = Color.cyan;
//                     break;
//                 case 10:
//                     ringColor = Color.white;
//                     break;
//                 case 1:
//                     ringColor = Color.grey;
//                     break;
//             }
//         }
//         StartCoroutine(DisappearingSequence());
//     }

//     IEnumerator DisappearingSequence()
//     {
//         //Destruction phase
//         while (targetScale.x < 0.53)
//         {
//             targetColor.a -= 0.1f;                 //Increase the opacity
//             targetSprite.color = targetColor;
//             if (noteColor.a > 0)
//             {
//                 noteColor.a -= 0.1f;                 //Increase the opacity
//                 noteSprite.color = noteColor;
//             }
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
