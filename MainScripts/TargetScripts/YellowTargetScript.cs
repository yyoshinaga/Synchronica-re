// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// using UnityEngine.UI;
// using TMPro;


// public class YellowTargetScript : MonoBehaviour
// {
//     //Bezier Curve stuff
//     public List<int[]> movingPoints;
//     private float goalBuffer;
//     GameObject lineObject;
//     public float mX;
//     public float mY;

//     public Transform[] curvePoints;
//     private Transform[] controlPoints;
//     public LineRenderer lineRenderer;
//     private Vector2[] linePixelArray;
//     private bool reachedGoal;

//     public int curveCount;
//     private int layerOrder;
//     private int SEGMENT_COUNT;
//     private int lineSplitter;

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
//     private bool beingTouched;
//     private int touchID;          // Finger ID

//     //Link
//     public GameObject link;

//     //Target 
//     private Transform Target;
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
//     private float fakeFillAmount;
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
//         goalBuffer = 10;
//         lineSplitter = 16;

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
//         moveNoteToPerfection = false;
//         reachedGoal = false;

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

//         //Only for yellow
//         bonusText = gameObject.transform.GetChild(7).gameObject;
//         bonusText.GetComponentInChildren<TMP_Text>().alpha = 0;

//         loader = gameObject.transform.GetChild(8).gameObject;
//         loader.GetComponentInChildren<CanvasGroup>().alpha = 0;
//         rightLoader = loader.GetComponentsInChildren<Image>()[0];
//         leftLoader = loader.GetComponentsInChildren<Image>()[1];
//         rightLoader.fillAmount = 0.5f;
//         leftLoader.fillAmount = 0.5f;
//         fakeFillAmount = rightLoader.fillAmount;
//         currentValue = 0;
//         //holdLength = 2.0f;

//         speed = 0.5f / holdLength;
//         explodeLoader = false;
//         released = false;

//         lineObject = transform.GetChild(9).gameObject;

//         //Audio stuff
//         audioSource = transform.GetComponent<AudioSource>();
//         hasNotPlayedSound = true;
//         hasNotPlayedEndSound = true;

//         DrawLineRenderer();

//         StartCoroutine(SimulateProjectile());
//     }
   
//     void DrawLineRenderer()
//     {
//         if (!lineRenderer)
//         {
//             lineRenderer = GetComponent<LineRenderer>();
//         }
//         lineRenderer.sortingLayerID = layerOrder;

//         curvePoints = new Transform[movingPoints.Count];
//         layerOrder = 0;
//         SEGMENT_COUNT = 48;

//         for (int i = 0; i < curvePoints.Length; i++)
//         {
//             curvePoints[i] = new GameObject().transform;

//             curvePoints[i].position = new Vector2(movingPoints[i][0]*mX, movingPoints[i][1]*mY);

//             curvePoints[i].parent = transform;
//             curvePoints[i].name = "Points";
//         }
//         if (curvePoints.Length != 2)
//         {
//             //curveCount = (int)controlPoints.Length / 3;
//             curveCount = curvePoints.Length - 1;
//             controlPoints = new Transform[(curvePoints.Length - 1) * 3 + 1];
        
//             //Curve points length = 3
//             for (int i = 0; i < curvePoints.Length; i++)
//             {
//                 controlPoints[i * 3] = curvePoints[i];  //Putting in the points into the controlPoints array
//             }

//             for (int i = 0; i < controlPoints.Length; i++)
//             {
//                 if (i % 3 != 0)
//                 {
//                     controlPoints[i] = new GameObject().transform;
//                     controlPoints[i].parent = transform;
//                     controlPoints[i].name = "Pivots";
//                 }
//             }
//             DrawCurve();
//         }
//         else
//         {
//             DrawLine();
//         }
//     }
//     void DrawLine()
//     {
//         lineRenderer.positionCount = 2;
//         lineRenderer.SetPosition(0, curvePoints[0].position);
//         lineRenderer.SetPosition(1, curvePoints[1].position);

//         Vector2 g = curvePoints[0].position;
//         Vector2 incre = (curvePoints[1].position - curvePoints[0].position) / SEGMENT_COUNT;

//         int linePixelArrayCounter = 0;
//         linePixelArray = new Vector2[SEGMENT_COUNT];
        
//         for (int i = 1; i <= SEGMENT_COUNT; i++)
//         {
//             linePixelArray[linePixelArrayCounter] = g + incre*i;
//             linePixelArrayCounter++;
//         }
//     }

//     void DrawCurve()
//     {
//         //j is controlPoints/3
//         for (int j = 1; j < curveCount; j++)
//         {
//             int nodeIndex = j * 3;
//             Vector2 dir = Vector2.zero;

//             //Left Point:
//             Vector2 offsetL = controlPoints[nodeIndex - 3].position - controlPoints[nodeIndex].position;
//             dir += offsetL.normalized;
//             float leftNeighbourDistances = offsetL.magnitude;

//             //Right Point:
//             Vector2 offsetR = controlPoints[nodeIndex + 3].position - controlPoints[nodeIndex].position;
//             dir -= offsetR.normalized;
//             float rightNeighbourDistances = -offsetR.magnitude;

//             dir.Normalize();

//             controlPoints[nodeIndex - 1].position = (Vector2)(controlPoints[nodeIndex].position) + dir * leftNeighbourDistances * .5f;
//             controlPoints[nodeIndex + 1].position = (Vector2)(controlPoints[nodeIndex].position) + dir * rightNeighbourDistances * .5f;
//         }

//         //DO THE FIRST SET OF POINTS
//         controlPoints[1].position = (Vector2)(controlPoints[0].position + controlPoints[2].position) * .5f;

//         //DO THE LAST SET OF POINTS
//         int max = controlPoints.Length;
//         controlPoints[max - 2].position = (Vector2)(controlPoints[max - 1].position + controlPoints[max - 3].position) * .5f;

//         int linePixelArrayCounter = 0;
//         linePixelArray = new Vector2[curveCount*SEGMENT_COUNT];

//         for (int j = 0; j < curveCount; j++)
//         {
//             for (int i = 1; i <= SEGMENT_COUNT; i++)
//             {
//                 float t = i / (float)SEGMENT_COUNT;
//                 int nodeIndex = j * 3;

//                 Vector2 pixel = CalculateCubicBezierPoint(t, controlPoints[nodeIndex].position, controlPoints[nodeIndex + 1].position, controlPoints[nodeIndex + 2].position, controlPoints[nodeIndex + 3].position);
//                 linePixelArray[linePixelArrayCounter] = pixel;
//                 linePixelArrayCounter++;

//                 lineRenderer.positionCount = (j * SEGMENT_COUNT) + i;
//                 lineRenderer.SetPosition((j * SEGMENT_COUNT) + (i - 1), pixel);
//             }
//         }
//     }

//     Vector2 CalculateCubicBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
//     {
//         float u = 1 - t;
//         float tt = t * t;
//         float uu = u * u;
//         float uuu = uu * u;
//         float ttt = tt * t;

//         Vector2 p = uuu * p0;
//         p += 3 * uu * t * p1;
//         p += 3 * u * tt * p2;
//         p += ttt * p3;

//         return p;
//     }

//     void Awake()
//     {
//         myTransform = transform.GetChild(0);
//     }

//     IEnumerator SimulateProjectile()
//     {
//         while (targetScale.x <= 0.5f && targetScale.y <= 0.5f)
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

//                 Note.Translate(-Vx * Time.deltaTime, 
//                     (Vy - (gravity * elapse_time)) * Time.deltaTime, 0);
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

//                 if (hit.collider != null)
//                 {
//                     if (targetIsReady && touch.phase == TouchPhase.Began)
//                     {
//                         if (hit.collider.transform.GetInstanceID() == gameObject.transform.GetInstanceID())  //Check to see if touch is colliding with target
//                         {

//                             if (hitSoundEnabled && hasNotPlayedSound)
//                             {
//                                 audioSource.PlayOneShot(audioSource.clip, 1f);
//                                 hasNotPlayedSound = false;
//                             }

//                             Destroy(link);
//                             MakeNoteMoveOrDisappear(moveNoteToPerfection);


//                             //Save the touchID for later
//                             touchID = touch.fingerId;
//                             miss = false;

//                             //Show the loader
//                             loader.GetComponentInChildren<CanvasGroup>().alpha = 1;
//                             //Rate the score
//                             if (!wordAppeared)
//                             {

//                                 wordAppeared = true;    //Prevent from happening twice
//                                 hasBeenHit = true;      //Start bursting the ring
//                                 SlightlyScaleDownTarget();                                      //When note is touched, the target gets scaled down a bit

//                                 if (!earlyTouch)
//                                 {
//                                     //Debug.Log("Early");
//                                     earlyText.GetComponentInChildren<Transform>().position = transform.position;
//                                     earlyText.GetComponentInChildren<TMP_Text>().alpha = 1;
//                                     touchScore = 0;
//                                     player.GetComponent<ScoreManager>().AddScore(touchScore, 4);

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
//                                     //Debug.Log("Slow");
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
//                     }
//                 }

//                 //If the finger is still within the touch boundary and touching the screen,
//                 if (!beingTouched && hasBeenHit && touch.fingerId == touchID && touch.phase != TouchPhase.Ended)
//                 {
//                     startTime = Time.time;
//                     beingTouched = true;
//                     Vector2 orig = transform.position;
//                     lineObject.transform.localPosition = new Vector2(-orig.x * (1 / 0.5f), -orig.y * (1 / 0.5f));

//                     StartCoroutine(LoadYellowCircle());

//                 }
            
//                 else if (hasBeenHit && touch.fingerId == touchID && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && !explodeLoader)
//                 {
//                     if (!released)
//                     {
//                         MoveYellowCircle(touchPosition);
//                     }
//                 }

//                 else if (hasBeenHit && touch.fingerId == touchID && (touch.phase == TouchPhase.Ended || explodeLoader))
//                 {
//                     //Stop Audio
//                     audioSource.Stop();

//                     //When touch ends
//                     released = true;

//                     //Stop the yellow circle loader and make it disappear
//                     loader.GetComponentInChildren<CanvasGroup>().alpha = 0;
//                     bonusText.GetComponentInChildren<Transform>().position = transform.position + new Vector3(0, 5f, 0);

//                     //Give score
//                     if (fakeFillAmount > 0.15 && fakeFillAmount <= 0.2 && reachedGoal)
//                     {
//                         bonusText.GetComponentInChildren<TMP_Text>().color = new Color32(180, 0, 255, 255);
//                         bonusText.GetComponentInChildren<TMP_Text>().alpha = 1;

//                         player.GetComponent<ScoreManager>().AddScore(5, 0);  //Essentially, adding 2 more points
//                     }
//                     else if (fakeFillAmount >= -0.1 && fakeFillAmount <= 0.15  && reachedGoal)
//                     {
//                         bonusText.GetComponentInChildren<TMP_Text>().color = new Color32(255, 255, 255, 255);
//                         bonusText.GetComponentInChildren<TMP_Text>().alpha = 1;

//                         player.GetComponent<ScoreManager>().AddScore(10, 0);  //Essentially, adding 10 more points
//                     }

//                     //Should put explodeLoader here in parameter but it's not working so...
//                     else if (reachedGoal)     //User went past the explosion point
//                     {
//                         bonusText.GetComponentInChildren<TMP_Text>().color = new Color32(180, 0, 255, 255);
//                         bonusText.GetComponentInChildren<TMP_Text>().alpha = 1;

//                         player.GetComponent<ScoreManager>().AddScore(5, 0);  //Essentially, adding 2 more points 
//                     }


//                     //Once the touch ends for , the game object goes into the destruction phase
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

//     void MoveYellowCircle(Vector2 touchPos)
//     {
//         //Find the closest point out of 32 divisions of pixelArray

//         float minDist = Mathf.Sqrt(Mathf.Pow(linePixelArray[0].x - touchPos.x,2)+Mathf.Pow(linePixelArray[0].y - touchPos.y,2));
//         int minPlacement = 0;
//         for (int i = linePixelArray.Length/ lineSplitter; i < linePixelArray.Length; i += linePixelArray.Length/ lineSplitter)
//         {
//             float contender = Mathf.Sqrt(Mathf.Pow(linePixelArray[i].x - touchPos.x,2)+ Mathf.Pow(linePixelArray[i].y - touchPos.y,2));

//             if (minDist > contender)
//             {
//                 minDist = contender;
//                 minPlacement = i;
//             }
//         }


//         float minDistAccurate = Mathf.Sqrt(Mathf.Pow(linePixelArray[minPlacement].x - touchPos.x,2)+ Mathf.Pow(linePixelArray[minPlacement].y - touchPos.y,2));
//         int minPlacementAccurate = minPlacement;

//         int forDebugging = 0;
//         try
//         {
//             for (int i = minPlacement + 1; i < minPlacement + Mathf.Floor(linePixelArray.Length / lineSplitter) - 1; i += 2)
//             {
//                 forDebugging = i;
//                 float contender = Mathf.Sqrt(Mathf.Pow(linePixelArray[i].x - touchPos.x, 2) + Mathf.Pow(linePixelArray[i].y - touchPos.y, 2));
//                 if (minDistAccurate > contender)
//                 {
//                     minDistAccurate = contender;
//                     minPlacementAccurate = i;
//                 }
//             }
//         }
//         catch
//         {
//             Debug.Log(minPlacement);
//             Debug.Log(Mathf.Floor(linePixelArray.Length / lineSplitter));
//             Debug.Log(forDebugging);
//             Debug.Log(linePixelArray.Length);
//         }
//         transform.position = linePixelArray[minPlacementAccurate];


//         if(myTransform.position.x < linePixelArray[linePixelArray.Length - 1].x + goalBuffer &&
//             myTransform.position.x > linePixelArray[linePixelArray.Length - 1].x - goalBuffer &&
//              myTransform.position.y < linePixelArray[linePixelArray.Length - 1].y + goalBuffer &&
//               myTransform.position.y > linePixelArray[linePixelArray.Length - 1].y - goalBuffer)
//         {
//             reachedGoal = true;
//         }
//     }

//     void DestructionPhase()
//     {
//         //Turn off collider to not disturb other notes underneath
//         gameObject.transform.GetComponent<CircleCollider2D>().enabled = false;

//         tooLateTouch = true;
//         Destroy(lineObject);
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
//                     case 5:
//                         ringColor = Color.yellow;
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
//         if (targetType == TargetType.Yellow && !miss)
//         {
//             StartCoroutine(LoadYellowCircle());
//         }
//         StartCoroutine(DisappearingSequence());
//     }

//     IEnumerator LoadYellowCircle()
//     {
//         fakeFillAmount = rightLoader.fillAmount;
//         while (fakeFillAmount >= -0.1 && !released) //Not 0.5 since the user has to stop at 0.5
//         {
//             currentValue = speed * (Time.time - startTime);
//             //Debug.Log(Time.time - startTime);
//             if (rightLoader.fillAmount > 0)
//             {
//                 rightLoader.fillAmount -= currentValue;
//                 leftLoader.fillAmount -= currentValue;
//             }
//             fakeFillAmount -= currentValue;
//             startTime = Time.time;
//             yield return null;
//         }
//         explodeLoader = true;
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

