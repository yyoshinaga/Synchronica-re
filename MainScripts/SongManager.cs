using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SongManager : MonoBehaviour {

    // Constants
    // Get screen resolution ratio from OSU 
    private const float osuX = 512f;
    private const float osuY = 384f; 
    private const float screenOffsetX = 0f;//-355f;
    private const float screenOffsetY = 0f;//-200f;
    private const float noteAppearanceDelay = -0.5f;
 

    private float multiplierX;
    private float multiplierY; 

    private GameObject gameManager;
    private GameObject background;
    private GameObject songName;
    private GameObject difficulty;
    // private GameObject linkInstance;
    private List<NoteData> noteSelect;
    private float delay = 0;
    private bool enableHitSound;
    private AudioSource myAudioSource;
    private int noteNumber = 0;  
    private Vector3 notePosition;
    private Quaternion noteRotation;
    private NoteData currentNote;
    private ReadBeatMap readBeatMap;

    private Canvas canvas;

    [SerializeField]
    protected GameObject BluePrefab;
    [SerializeField]
    protected GameObject YellowPrefab;
    [SerializeField]
    protected GameObject GreenPrefab;
    [SerializeField]
    protected GameObject RedPrefab;
    [SerializeField]
    protected GameObject LinkPrefab;

    private IEnumerator Start() {
        gameManager = GameObject.FindWithTag("GameManager");
        background = GameObject.FindWithTag("Background");
        songName = GameObject.FindWithTag("SongName");
        difficulty = GameObject.FindWithTag("Difficulty");

        canvas = transform.parent.GetComponent<Canvas>();

        // Temporary debug
        SelectedSong.SetBackgroundFile("Debug/BG");

        // Fetch the background file and set it as sprite
        Sprite backgroundSprite = null;
        if(SelectedSong.backgroundFile != null)
        {
            backgroundSprite = 
                Resources.Load<Sprite>(SelectedSong.backgroundFile);
        }
        background.GetComponent<SpriteRenderer>().sprite = backgroundSprite;

        background.transform.position = new Vector3(0,0,0);

        // Set song name and difficulty
        songName.GetComponent<TMP_Text>().text = SelectedSong.songTitle;
        difficulty.GetComponent<TMP_Text>().text = SelectedSong.difficulty;
        
        // Set hitsound settings
        enableHitSound = SelectedSong.enableHitSound;

        // Fetch the AudioSource from the GameObject
        myAudioSource = gameManager.GetComponent<AudioSource>();
        Debug.Assert(myAudioSource != null, "myAudioSource is null");

        readBeatMap = gameManager.GetComponent<ReadBeatMap>();
        Debug.Assert(readBeatMap != null, "readBeatMap is null");

        // Fetch the beatmap list from ReadBeatMap
        noteSelect = readBeatMap.Read();
        Debug.Assert(noteSelect.Count > 0, "noteSelect count is 0");

        // Select between delay from GUI or delay from beatmap
        if (SelectedSong.delay == 0)
        {
            delay = readBeatMap.GetDelay();
        }
        else
        {
            delay = SelectedSong.delay;
        }

        // Set default rotation for green note
        noteRotation = Quaternion.Euler(0, 0, 0);

        // Initialize multiplier using screen size and osu size
        multiplierX = (float)Screen.width / osuX;
        multiplierY = (float)Screen.height / osuY;


        // Wait few seconds before playing song
        yield return new WaitForSeconds(3);

        // Start audio
        myAudioSource.Play();

        // Load asynchronously
        // yield return LoadSceneAsync(2);
    }

    // private IEnumerator LoadSceneAsync(int sceneIndex)
    // {
    //     AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);

    //     // Optional: Prevent the scene from activating until loading is complete
    //     asyncOperation.allowSceneActivation = false;

    //     while (!asyncOperation.isDone)
    //     {
    //         Debug.Log($"Loading progress: {asyncOperation.progress * 100}%");

    //         // When loading is complete (progress reaches 0.9), activate the scene
    //         if (asyncOperation.progress >= 0.9f)
    //         {
    //             Debug.Log("Scene loaded. Activating...");
    //             asyncOperation.allowSceneActivation = true;
    //         }

    //         yield return null; // Wait for the next frame
    //     }
    //     yield return null;
    // }

    void FixedUpdate()
    {
        // If the song is over, return
        if (noteNumber > noteSelect.Count) 
        {
            Debug.Log("No more notes");
            return;
        }
        
        // Get a note from the beatmap list
        currentNote = noteSelect[noteNumber];
        float noteTime = currentNote.GetTime() / 1000.0f;
        float time = myAudioSource.time;

        // Check if its time to show the note
        if (time >= (noteTime - delay + noteAppearanceDelay))
        {
            // Initialize note position with multiplier
            notePosition = new Vector3(
                multiplierX * currentNote.GetX() + screenOffsetX, 
                multiplierY * currentNote.GetY() + screenOffsetY, 
                0f
            );
            
            GameObject linkInstance = null;
            if (currentNote.hasLinks)
            {
                linkInstance = CreateLink();
            }

            // Get type of note
            NoteType noteType = currentNote.GetNoteType(); 

            // Create an instance of GameObject and attach note
            GameObject noteInstance;
            switch (noteType)
            {
                case NoteType.BLUE:
                    noteInstance = CreateBlueNote(linkInstance);
                    break;
                case NoteType.GREEN:
                    noteInstance = CreateRedNote(linkInstance);
                    break;
                case NoteType.RED:
                    noteInstance = CreateGreenNote(linkInstance);
                    break;
                case NoteType.YELLOW:
                    noteInstance = CreateYellowNote(linkInstance);
                    break;
                default:
                    throw new Exception(
                        "There's an error in the beatmap types: " + noteType);
            }

            // Increase count of notes
            noteNumber++;
        }        
    }

    private GameObject CreateLink()
    {
        GameObject linkInstance = Instantiate(LinkPrefab, Vector3.zero, noteRotation);
        linkInstance.transform.SetParent(canvas.transform);

        Links linkComponent = linkInstance.GetComponent<Links>();

        linkComponent.point1 = new Vector2(
            currentNote.GetLinks()[0][0] * multiplierX + screenOffsetX,
            currentNote.GetLinks()[0][1] * multiplierY + screenOffsetY
        );
        linkComponent.point2 = new Vector2(
            currentNote.GetLinks()[1][0] * multiplierX + screenOffsetX,
            currentNote.GetLinks()[1][1] * multiplierY + screenOffsetY
        );
        return linkInstance;
    }

    private GameObject CreateBlueNote(GameObject linkInstance)
    {
        GameObject blueNote = 
            Instantiate(BluePrefab, notePosition, noteRotation);
        blueNote.transform.SetParent(canvas.transform);

        BlueNote blueNoteScript = blueNote.GetComponent<BlueNote>();

        blueNoteScript.link = linkInstance;
        Debug.Log($"is link nu.l? : {linkInstance == null}");
        blueNoteScript.textHandler = blueNote.GetComponent<TextHandler>();

        return blueNote;
    }

    private GameObject CreateRedNote(GameObject linkInstance)
    {
        GameObject redNote = 
            Instantiate(RedPrefab, notePosition, noteRotation);
        redNote.transform.SetParent(canvas.transform);

        RedNote redNoteScript = redNote.GetComponent<RedNote>();

        redNoteScript.holdLength = 
            ((float)currentNote.GetEndTime() - (float)currentNote.GetTime()) / 
            1000f;
        redNoteScript.link = linkInstance;
        redNoteScript.textHandler = redNote.GetComponent<TextHandler>();

        return redNote;
    }
    private GameObject CreateGreenNote(GameObject linkInstance)
    {
        float rotation = currentNote.GetAngle();
        Quaternion greenRotation = Quaternion.Euler(180, 0, rotation - 90);

        GameObject greenNote = 
            Instantiate(GreenPrefab, notePosition, noteRotation);
        greenNote.transform.SetParent(canvas.transform);

        greenNote.transform.GetChild(0).rotation = greenRotation;

        GreenNote greenNoteScript = greenNote.GetComponent<GreenNote>();
        greenNoteScript.direction = rotation + 90;
        greenNoteScript.link = linkInstance;
        greenNoteScript.textHandler = greenNote.GetComponent<TextHandler>();

        return greenNote;
    }
    private GameObject CreateYellowNote(GameObject linkInstance)
    {
        GameObject yellowNote = 
            Instantiate(YellowPrefab, notePosition, noteRotation);
        yellowNote.transform.SetParent(canvas.transform);

        YellowNote yellowNoteScript = yellowNote.GetComponent<YellowNote>();

        List<int[]> controlPoints = currentNote.GetPoints();

        // This will technically modify the original points in the currentNote
        // But who cares. It saves time+memory from creating a deepcopy
        foreach (int[] point in controlPoints)
        {
            point[0] = (int)Math.Round(point[0] * multiplierX + screenOffsetX);
            point[1] = (int)Math.Round(point[1] * multiplierY + screenOffsetY);
        }
        yellowNoteScript.controlPoints = controlPoints;
        yellowNoteScript.holdLength = 
            ((float)currentNote.GetEndTime() - (float)currentNote.GetTime()) / 
            1000f;
        yellowNoteScript.link = linkInstance;
        yellowNoteScript.textHandler = yellowNote.GetComponent<TextHandler>();

        return yellowNote;
    }
}