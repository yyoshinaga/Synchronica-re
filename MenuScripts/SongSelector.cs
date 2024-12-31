using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq; // ICollection ToList()


// Architecture

// Rules: 
// 1. Selected song should show up on the left, 
//   menu should show on the right to avoid clutter UI
// 2. No multiple difficulty setting
// 3. Song list must be read through during runtime to allow modification
//      of list in realtime 

// SelectedSong -> single unit of data, w/ access methods
// SongSelector -> 
    // GetSongList() -> Must load from txt file 
    // ToNextPage() -> Repopulate menu to next 9 entries
    // ToPrevPage() -> Repopulate menu to prev 9 entries
    // SelectGenre() -> move page over to specific genre
    // EnableHitSound()
    // EnableBackground()
    // DelaySong() 
     
// Example json file
// {
//  "songs": [
//      {
//          "title": "blahblah",
//          "folder": "somewhere",
//          "genre": "something"
//      }
// ]
// }

// Set up the json file schema
[System.Serializable] // Need this for json to be parsed
public class SongInfo
{
    public string title;
    public string folder;
    public string genre; // Keep as string since json uses string, not enum
}

[System.Serializable] // Need this for json to be parsed
public class SongInfos
{
    public SongInfo[] songInfos;
}

public enum Genre { ROCK, POP, JPOP, ANIME, CLASSIC, KPOP, RANDOM}


public class SongSelector : MonoBehaviour
{
    // Do not change this constant unless you change the GUI as well
    private const int maxTrackPerPage = 9; 

    // Private variables
    private int totalIds;
    private int currentId;
    private int currentPage;
    private int totalPages;

    // Contains all songs in the form: { ID: SongInfo }
    private Dictionary<int, SongInfo> songEntries;
    private List<Transform> trackList; // A track is the button to select songs
    // private Dictionary<int, int> trackToId;
    private Dictionary<int, int[]> pageToId;
    private TextMeshProUGUI delayValueText;

    public Transform enableHitSound;
    public Transform enableBackground;
    public Transform delayTime;
    public TMP_Dropdown dropDownMenu;
    public Transform menuItems;
    public Transform delayValue;

    public TextMeshProUGUI chosenSongText;
    public Transform chosenSongImage;
    public Transform nextPage;
    public Transform prevPage;


    // Start is called before the first frame update
    void Start()
    {
        // Initialize
        trackList = new List<Transform>();
        pageToId = new Dictionary<int, int[]>();
        songEntries = new Dictionary<int, SongInfo>();
        currentPage = 0;
        delayValueText = delayValue.GetComponentInChildren<TextMeshProUGUI>();

        // Loop through all buttons and add listeners
        for (int i = 0; i < maxTrackPerPage; i++)
        {
            // Find the track object 
            Transform track = menuItems.Find($"Track{i}");

            // Pass the ID for each button to the listener
            int index = i;  // Local copy of the index to avoid closure issues
            track.GetComponent<Button>().onClick.AddListener(() => OnTrackButtonClicked(index));

            // Add to list
            trackList.Add(track);
        }

        // Set up next and prev page buttons
        nextPage.GetComponent<Button>().onClick.AddListener(() => OnNextButtonClicked());
        prevPage.GetComponent<Button>().onClick.AddListener(() => OnPrevButtonClicked());

        // Set up checkbox
        enableHitSound.GetComponent<Toggle>().onValueChanged.AddListener(OnEnableHitSound);
        enableBackground.GetComponent<Toggle>().onValueChanged.AddListener(OnEnableBackground);

        // Set up slider
        Slider slider = delayTime.GetComponent<Slider>();
        slider.minValue = -1f;
        slider.maxValue = 1f;
        slider.value = 0f;
        slider.onValueChanged.AddListener(OnDelayValueChanged);

        // Run this function whenever a new song has been inserted
        ReadSongList();

        // Populate the dropdown
        PopulateGenre();

        // Initialize pages
        InitializePages();

        // Populate the tracks
        PopulateTracks();
    }

    // Private methods
    private void PopulateGenre()
    {
        // Clear the drop down list before repopulating
        dropDownMenu.ClearOptions();

        // Add in each genre from the list
         // Get enum values as an array
        Array enumValues = Enum.GetValues(typeof(Genre));

        // Create a list to store the option strings
        List<string> options = new List<string>();

        // Loop through the enum values and add their names to the list
        foreach (Genre value in enumValues)
        {
            string genreString = value.ToString();

            // Make all the enum first letter caps and then all other lower
            genreString = char.ToUpper(genreString[0]) + genreString.Substring(1).ToLower();

            // Add string
            options.Add(genreString); // Add the name of the enum value as string
        }

        // Add entire genre string list to dropdown
        dropDownMenu.AddOptions(options);

        // Initialize to first option
        dropDownMenu.value = 0;
    }

    private void InitializePages()
    {
        // Get genre of first entry
        SongInfo entry = songEntries[0];
        Genre currentGenre = stringToGenre(entry.genre);

        // Get all keys from the dictionary
        List<int> songIds = songEntries.Keys.ToList();

        int pageNumber = 0;
        int songIndex = 0; // ID index

        // Loop through all the IDs
        while (songIndex < songIds.Count)
        {
            // Create a new array with size: maxTrackPerPage
            int[] pageIds = new int[maxTrackPerPage];

            // Fill in all tracks with -1 
            for (int i = 0; i < maxTrackPerPage; i++)
            {
                pageIds[i] = -1;
            }

            for (int i = 0; i < maxTrackPerPage; i++)
            {
                // Get an ID
                int songId = songIds[songIndex];

                // Find the SongInfo corresponding to ID
                entry = songEntries[songId];
                
                // Check if current entry genre is the page's genre
                if (stringToGenre(entry.genre) == currentGenre)
                {
                    // Store the songID into the array
                    pageIds[i] = songId;
                    
                    // Only go to next ID if song was put into page
                    songIndex += 1;

                    // Break out of the for loop if reached end of song IDs
                    if (songIndex >= songIds.Count)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            // Add array to the page
            pageToId.Add(pageNumber, (int[])pageIds.Clone());

            // Change page number
            pageNumber += 1;
            
            // Change genre
            currentGenre = stringToGenre(entry.genre);
        }

        // Total pages
        totalPages = pageNumber;
    }

    private Genre stringToGenre(string genreString)
    {
        genreString = genreString.ToUpper();
        
        Genre genre;

        // Attempt to parse the string into the enum
        bool status = Enum.TryParse(genreString, out genre);
        Debug.Assert(status, $"Failed to parse '{genreString}' into a valid Genre enum.");

        return genre;
    }

    // SongInfo is a class declared in this file (above)
    private void SetTrack(int trackNumber, int id)
    {
        Debug.Assert(0 <= trackNumber && trackNumber < maxTrackPerPage, 
                        "Track number exceeds max track per page");

        // Get the specific track from the list
        Transform track = trackList[trackNumber];

        // The image of the track
        Image trackImage = track.GetChild(0).GetComponent<Image>();
        trackImage.sprite = null;

        // The title of the track
        TMP_Text trackTitle = track.GetChild(1).GetComponent<TMP_Text>();
        trackTitle.text = "";

        // Fill in track info
        if (id != -1)
        {
            SongInfo songInfo = songEntries[id];

            string backgroundPath = songInfo.folder + "/BG";
            trackImage.sprite = Resources.Load<Sprite>(backgroundPath);

            trackTitle.text = songInfo.title;
        }
    }

    private void PopulateTracks()
    {
        // Fill out all info 
        for (int i = 0; i < maxTrackPerPage; i++)
        {
            // Clear track before setting it
            SetTrack(i, -1);

            // Get the next ID
            currentId = pageToId[currentPage][i];

            if (currentId != -1)
            {
                // Set current song info to track
                SetTrack(i, currentId);                    
            }
        }
    }

    private void ReadSongList()
    {
        // File path to the JSON file in StreamingAssets
        string filePath = Path.Combine(Application.streamingAssetsPath, "SongList.json");

        // Read the file (use UnityWebRequest for Android or other platforms)
        string json = File.ReadAllText(filePath);

        // Read the json file
        SongInfo[] songInfos = JsonUtility.FromJson<SongInfos>(json).songInfos;

        Debug.Assert(songInfos.Length > 0, "json file has no songs");

        // Sorting by genre first, then by title
        Array.Sort(songInfos, (song1, song2) =>
        {
            // First compare by genre
            int genreComparison = song1.genre.CompareTo(song2.genre);
            if (genreComparison == 0)
            {
                // If genres are the same, compare by title
                return song1.title.CompareTo(song2.title);
            }
            return genreComparison;
        });

        // Create a unique ID
        int id = 0;

        // Loop through all elements of the json file
        foreach (SongInfo songInfo in songInfos)
        {
            // Add entry to the list
            songEntries.Add(id, songInfo);

            // Assign unique ID in the order they appear in the json
            // The reason for not assigning directly in the json file is because
            // you can update the json file while playing the game
            id += 1;
        }
        // Store total number of existing ids
        totalIds = id + 1;

        // Set current ID to the first entry
        currentId = 0;
    }

    // Public methods
    public void OnTrackButtonClicked(int trackNumber)
    {
        // Set selected song image as the image of the button
        Transform track = trackList[trackNumber];

        int id = pageToId[currentPage][trackNumber];

        if (id != -1)
        {
            // Set selected id to button id
            SelectSong(id);
                        
            // Set the selected song image
            Sprite s = track.GetChild(0).GetComponent<Image>().sprite;
            chosenSongImage.GetComponent<Image>().sprite = s;

            // Set the selected song title
            chosenSongText.text = track.GetChild(1).GetComponent<TMP_Text>().text;
        }
    }

    public void OnNextButtonClicked()
    {
        currentPage = (currentPage + 1) % totalPages;

        PopulateTracks();
    }

    public void OnPrevButtonClicked()
    {
        currentPage = (totalPages - currentPage - 1) % totalPages;

        PopulateTracks();
    }

    public void SelectSong(int id)
    {
        // Fill out information
        SelectedSong.songID = id;
        SelectedSong.songTitle = songEntries[id].title;
        SelectedSong.songFolder = songEntries[id].folder;
        SelectedSong.genre = songEntries[id].genre;

        // Default naming/locations
        SelectedSong.beatMapFilePath = "beatmap.txt";
        SelectedSong.audioFilePath = "audio.mp3";
        SelectedSong.backgroundFile = "BG"; // TODO: jpg or png

        // Default values
        SelectedSong.delay = 0f;
        SelectedSong.enableBackground = true;
        SelectedSong.enableHitSound = true;
    }

    public void SelectGenre()
    {
        // Change the page to the selected genre

        
    }

    public void OnEnableBackground(bool enabled)
    {
        SelectedSong.enableBackground = enabled;
    }

    public void OnEnableHitSound(bool enabled)
    {
        SelectedSong.enableHitSound = enabled;
    }

    public void OnDelayValueChanged(float value)
    {
        // Round to nearest first decimal
        value = Mathf.Round(value * 10f) / 10f;

        // Set value
        SelectedSong.delay = value;

        // Set GUI 
        delayValueText.text = value.ToString();
    }
}
