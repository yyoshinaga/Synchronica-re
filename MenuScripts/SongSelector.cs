using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;
using TMPro;


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
public class SongInfo
{
    public string title;
    public string folder;
    public string genre;
}

public class SongInfos
{
    public SongInfo[] songInfos;
}


public class SongSelector : MonoBehaviour
{
    private const int maxSongPerPage = 9; 

    // { ID: SongInfo }
    private Dictionary<int, SongInfo> songEntries;

    private TextMeshProUGUI delayValue;
    private TextMeshProUGUI genreText;
    private GameObject songSelectMenu;
    private TMP_Dropdown dropDownMenu;

    private List<string> genreList;
    private List<GameObject> songSelectButtons;



    // Start is called before the first frame update
    void Start()
    {
        
        // Song delay value
        delayValue = GameObject.FindWithTag("DelayValue").GetComponent<TextMeshProUGUI>();
        // delayValue.text = "1.0"; // Why is it set to 1.0?

        // Song select menu
        songSelectMenu = GameObject.FindWithTag("SongSelectMenu");

        // Dropdown Menu
        dropDownMenu = GameObject.FindWithTag("DropDownMenu").GetComponent<TMP_Dropdown>();

        



        // Run this function whenever a new song has been inserted
        PopulateGenre();

        ReadSongList();
    }

    // Private methods

    private void PopulateGenre()
    {
        // Clear the drop down list before repopulating
        dropDownMenu.options.Clear();

        // Add in each genre from the list
        foreach (string genre in genreList)
        {
            dropDownMenu.options.Add(new TMP_Dropdown.OptionData() { text = genre });
        }
    }

    private void PopulateSongImages()
    {
        // Clear everything 
        for (int i = 0; i < maxSongPerPage; i++)
        {
            SongInfo info = songSelectButtons[i].GetComponent<SongInfo>();



        }

    }

    private Dictionary<int, SongInfo> ReadSongList()
    {
        // Load text file
        TextAsset jsonFile = Resources.Load<TextAsset>("SongList") as TextAsset;
 
        // Read the json file
        SongInfos songsInJson = JsonUtility.FromJson<SongInfos>(jsonFile.text);

        // Create a unique ID
        int id = 0;

        // Loop through all elements of the json file
        foreach (SongInfo songInfo in songsInJson.songInfos)
            // Add entry to the list
            songEntries.Add(id, songInfo);

            // Assign unique ID in the order they appear in the json
            // The reason for not assigning directly in the json file is because
            // you can update the json file while playing the game
            id += 1;

        return songEntries;
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


    // Public methods

    public void SelectGenre()
    {
        // Change the page to the selected genre

        
    }

    public void EnableBackground(bool enable)
    {
        SelectedSong.enableBackground = enable;
    }

    public void EnableHitSound(bool enable)
    {
        SelectedSong.enableHitSound = enable;
    }

    public void DelaySong(float value)
    {
        // Round to nearest first decimal
        SelectedSong.delay = Mathf.Round(value * 10f) / 10f;

        // Change the GUI text
        delayValue.text = value.ToString();
    }
}
