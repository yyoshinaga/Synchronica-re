// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;
// using System;
// using UnityEngine.UI;
// using TMPro;

// public class LevelImage : MonoBehaviour {
//     private List<string> readList;
//     private int lineIndex;
//     private int charIndex;

//     public Dictionary<string, ArrayList> completeSongList;
//     public string Genre;
//     public int PageNumber;

//     private int currentPageNumber;
//     private int currentGenreNumber;
//     List<string> genreList;

//     private GameObject Track1;
//     private GameObject Track2;
//     private GameObject Track3;
//     private GameObject Track4;
//     private GameObject Track5;
//     private GameObject Track6;
//     private GameObject Track7;
//     private GameObject Track8;
//     private GameObject Track9;
//     private List<GameObject> trackList;

//     private GameObject easy;
//     private GameObject normal;
//     private GameObject hard;

//     private TextMeshProUGUI delayValue;

//     private TextMeshProUGUI genreText;

//     private string tempBG;

//     private GameObject difficultySelectMenu;
//     private GameObject songSelectMenu;
//     private TMP_Dropdown dropDownMenu;

//     //Goal: 

//     //Get txt file with song list
//     //Get reference folder with all songs
//     //Get the song in the list and place the image file in the correct spot
//     void Start()
//     {
//         easy = GameObject.FindWithTag("EasyButton");
//         normal = GameObject.FindWithTag("NormalButton");
//         hard = GameObject.FindWithTag("HardButton");

//         delayValue = GameObject.FindWithTag("DelayValue").GetComponent<TextMeshProUGUI>();
//         delayValue.text = "1.0";

//         songSelectMenu = GameObject.FindWithTag("SongSelectMenu");

//         dropDownMenu = GameObject.FindWithTag("DropDownMenu").GetComponent<TMP_Dropdown>();

//         difficultySelectMenu = GameObject.FindWithTag("DifficultySelectMenu");
//         difficultySelectMenu.SetActive(false);

//         completeSongList = new Dictionary<string, ArrayList>();

//         // Find all the tracks
//         Transform menuItemsTransform = GameObject.FindWithTag("MenuItems").transform;
//         trackList = new List<GameObject>();
//         for (int i = 1; i <= 9; i++)
//         {
//             trackList.Add(menuItemsTransform.Find($"Track{i}").gameObject);
//         }

//         //First read the text file
//         ReadSongList();

//         //Fill in the genreList
//         genreList = new List<string>(completeSongList.Keys);
        
//         //Populate the dropdown bar
//         PopulateDropDown();

//         currentPageNumber = 0;
//         currentGenreNumber = 0;
//         GenerateImageList(genreList[currentGenreNumber], currentPageNumber);
//     }

//     private List<string> TextAssetToList(TextAsset ta)
//     {
//         return new List<string>(ta.text.Split('\n'));
//     }

//     public Dictionary<string, ArrayList> ReadSongList()
//     {
//         TextAsset textFile = Resources.Load<TextAsset>("SongList") as TextAsset;
//         readList = TextAssetToList(textFile);

//         lineIndex = 0;
//         charIndex = 0;
//         char c = readList[lineIndex][charIndex];
//         charIndex++;

//         int numSpaces = 0;
//         string[] array = new string[6];
//         List<string[]> page = new List<string[]>();
//         ArrayList currentGenreArray = new ArrayList();
//         string currentGenreName = "";

//         bool firstGenre = true;
 
//         int tryCounter = 0;
//         while (lineIndex <= readList.Count-1 && tryCounter < 4000)
//         {
//             tryCounter++;
//             if(c == '[')
//             {
//                 if (!firstGenre)             //If the page doesn't fill up, its still okay
//                 {
//                     currentGenreArray.Add(page);            //Add existing page onto genre
//                     page = new List<string[]>();            //Start a fresh page
//                     array = new string[6];                  //Refresh the array
//                     completeSongList.Add(currentGenreName, currentGenreArray);
//                 }
//                 else
//                 {
//                     firstGenre = false;
//                 }
//                 currentGenreName = "";

//                 c = ToNextChar(lineIndex, charIndex);
//                 while (c != ']')
//                 {
//                     currentGenreName += c;    //Make the currentGenreName the name in []
//                     charIndex++;
//                     c = ToNextChar(lineIndex, charIndex);
//                 }

//                 currentGenreArray = new ArrayList();        //Reset genre array list
//                 charIndex = 0;
//                 lineIndex++;
//                 c = ToNextChar(lineIndex, charIndex);
                
//             }
//             else if (c == '\n')  //if \n, then at end of note info, so add to list, then make new array
//             {
//                 lineIndex++;
//                 charIndex = 0;

//                 //Make sure first letter is a digit or an alphabet
//                 if(System.Char.IsLetter((array[0].ToCharArray())[0]) || System.Char.IsDigit((array[0].ToCharArray())[0]))
//                 {

//                     //--------------------------------------ATTENTION----------------------------------
//                     //If there's a problem with image, make sure to put a space on the last row of the songlist...


//                     //I believe this below is a string conversion...
//                     string something = array[5];
//                     array[5] = something.Substring(0, something.Length - 1);

//                     //Debug.Log(array[5]);
//                     numSpaces = 0;

//                     if (page.Count < 9)                         //When there's less than 9 items on a page
//                     {
//                         page.Add(array);                        //Add a song onto the page
//                         array = new string[6];                  //Refresh the array
//                     }
//                     else                                        //When the page count is over 9
//                     {
//                         currentGenreArray.Add(page);            //Add existing page onto genre
//                         page = new List<string[]>();            //Start a fresh page
//                         page.Add(array);                        //Add the first array in the page
//                         array = new string[6];                  //Refresh the array
//                     }
//                 }
//                 else
//                 {
//                     Debug.Log("Your song title has to start with a letter or a number");
//                 }
              
//             }
//             else if (c == '|')
//             {
//                 numSpaces++;
//             }
//             else
//             {
//                 array[numSpaces] += c;
//             }

//             c = ToNextChar(lineIndex, charIndex);
//             charIndex++;
//         }
//         if(tryCounter == 4000)
//         {
//             Debug.Log("Most likely, you have more than two of the same songs, sucker\nOr, increase the tryCounter");
//         }

//         //add final array, because it doesn't happen automatically from loop
//         currentGenreArray.Add(page);            //Add existing page onto genre
//         completeSongList.Add(currentGenreName, currentGenreArray);
        
//         return completeSongList;
//     }

//     private char ToNextChar(int lineIndex, int charIndex)
//     {
//         try
//         {
//             return readList[lineIndex][charIndex];
//         }
//         catch
//         {
//             return '\n';
//         }
//     }

//     public void SetInfo(int i, string[] song)
//     {
//         trackList[i].GetComponent<SongInfo>().ButtonID = i;
//         trackList[i].GetComponent<SongInfo>().SongFolder = song[0];
//         trackList[i].GetComponent<SongInfo>().SongTitle = song[1];
//         trackList[i].GetComponent<SongInfo>().Difficulty = song[2];
//         trackList[i].GetComponent<SongInfo>().beatMapFile = song[0] + '/' + song[3];
//         trackList[i].GetComponent<SongInfo>().AudioFile = song[0] + '/' + song[4];
//         trackList[i].GetComponent<SongInfo>().BGFile = song[0] + '/' + song[5];
//     }

//     public void NextPage()
//     {
//         ClearImageList();

//         try
//         {
//             currentPageNumber++;
//             GenerateImageList(genreList[currentGenreNumber], currentPageNumber);
//         }
//         catch
//         {
//             currentGenreNumber++;
//             currentPageNumber = 0;
//             if (genreList.Count > currentGenreNumber)
//             {
//                 GenerateImageList(genreList[currentGenreNumber], currentPageNumber);
//             }
//             else
//             {
//                 currentGenreNumber = 0;
//                 GenerateImageList(genreList[currentGenreNumber], currentPageNumber);
//             }
//         }
//     }

//     public void PrevPage()
//     {
//         ClearImageList();
//         try
//         {
//             currentPageNumber--;
//             GenerateImageList(genreList[currentGenreNumber], currentPageNumber);
//         }
//         catch
//         {
//             currentGenreNumber--;

//             if (currentGenreNumber >= 0)
//             {
//                 currentPageNumber = completeSongList[genreList[currentGenreNumber]].Count-1;
//                 GenerateImageList(genreList[currentGenreNumber], currentPageNumber);
//             }
//             else
//             {
//                 currentGenreNumber = genreList.Count - 1;
//                 currentPageNumber = completeSongList[genreList[currentGenreNumber]].Count-1;
//                 GenerateImageList(genreList[currentGenreNumber], currentPageNumber);
//             }
//         }
//     }

//     public void ChangeGenre(int genrePage)
//     {
//         ClearImageList();
        
//         currentPageNumber = 0;
//         currentGenreNumber = genrePage;
//         GenerateImageList(genreList[currentGenreNumber], currentPageNumber);
//     }

//     public void ClearInfo(int i)
//     {
//         trackList[i].GetComponent<SongInfo>().ButtonID = 0;
//         trackList[i].GetComponent<SongInfo>().SongFolder = "";
//         trackList[i].GetComponent<SongInfo>().SongTitle = "";
//         trackList[i].GetComponent<SongInfo>().Difficulty = "";
//         trackList[i].GetComponent<SongInfo>().NoteFile = "";
//         trackList[i].GetComponent<SongInfo>().AudioFile = "";
//         trackList[i].GetComponent<SongInfo>().BGFile = "";
//     }

//     public void ClearImageList()
//     {
//         for (int i = 0; i < 9; i++)
//         {
//             ClearInfo(i);               //Sets the information about the song
//             GameObject imageComponent = trackList[i].transform.GetChild(0).gameObject;
//             imageComponent.GetComponent<Image>().sprite = null;
//             TMP_Text levelText = trackList[i].transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
//             levelText.text = "";

//             TMP_Text titleText = trackList[i].transform.GetChild(2).gameObject.GetComponent<TMP_Text>();
//             titleText.text = "";

//             genreText = GameObject.FindWithTag("GenreText").GetComponent<TextMeshProUGUI>();
//             genreText.text = "";
//         }
//     }

//     public void GenerateImageList(string genreName, int pageNumber)
//     {
//         string imagePath = "";

//         ArrayList currentGenreArray = completeSongList[genreName];

//         List<string[]> page = (List<string[]>)currentGenreArray[pageNumber];        //Grabbing a page 

//         //Amount of songs on a page
//         for (int i = 0; i < page.Count; i++) {

//             string[] song = page[i];        //The ith song on the page

//             SetInfo(i, song);               //Sets the information about the song

//             imagePath = song[0] + '/' + song[5];
//             Texture2D myTexture = Resources.Load(imagePath) as Texture2D;
        
//             GameObject imageComponent = trackList[i].transform.GetChild(0).gameObject; //This is the image

//             //This is a debugger for image paths
            
//             char[] c = imagePath.ToCharArray();
//             /*
//             for(int j = 0; j < c.Length; j++)
//             {
//                 Debug.Log(c[j]);
//             }
//             */
            
//             imageComponent.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath);

//             TMP_Text levelText = trackList[i].transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
//             levelText.text = song[2];

//             TMP_Text titleText = trackList[i].transform.GetChild(2).gameObject.GetComponent<TMP_Text>();
//             titleText.text = song[1];

//             genreText = GameObject.FindWithTag("GenreText").GetComponent<TextMeshProUGUI>();
//             genreText.text = genreName;
//         }
//     }

//     private void PopulateDropDown()
//     {
//         dropDownMenu.options.Clear();

//         foreach (string t in genreList)
//         {
//             dropDownMenu.options.Add(new TMP_Dropdown.OptionData() { text = t });
//         }
//     }

//     public void ButtonClick(GameObject clickedButton)
//     {
  
//         if (!clickedButton.GetComponent<SongInfo>().SongTitle.Equals(""))
//         {
//             GameObject c = clickedButton;
//             SelectedSong.songTitle = clickedButton.GetComponent<SongInfo>().SongTitle;
//             SelectedSong.songFolder = clickedButton.GetComponent<SongInfo>().SongFolder;
//             SelectedSong.difficulty = clickedButton.GetComponent<SongInfo>().Difficulty;
//             SelectedSong.noteFile = clickedButton.GetComponent<SongInfo>().NoteFile;
//             SelectedSong.audioFile = clickedButton.GetComponent<SongInfo>().AudioFile;
//             SelectedSong.backgroundFile = clickedButton.GetComponent<SongInfo>().BGFile;

//             SelectedSong.delay = clickedButton.GetComponent<SongInfo>().Delay;
//             tempBG = clickedButton.GetComponent<SongInfo>().BGFile;
//             //SelectedSong.getTitle();
//             SelectedSong.enableHitSound = true;

//             difficultySelectMenu.SetActive(true);
//             songSelectMenu.SetActive(false);

//             c = GameObject.FindWithTag("MainImage");

//             c.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = 
//             clickedButton.transform.GetChild(0).GetComponent<Image>().sprite;
//             TMP_Text t = c.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
//             t.text = clickedButton.transform.GetChild(2).GetComponent<TMP_Text>().text;

//         }
//     }

//     public void GoBackToSongSelection()
//     {
//         songSelectMenu.SetActive(true);
//         difficultySelectMenu.SetActive(false);
//     }

//     public void DisableBackground(bool enable)
//     {
//         if (enable)
//         {
//             SelectedSong.backgroundFile = tempBG;
//             Debug.Log("Background enabled");
//         }
//         else
//         {
//             SelectedSong.backgroundFile = null;
//             Debug.Log("Background disabled");
//         }
//     }

//     public void EnableHitSound(bool enable)
//     {
//         SelectedSong.enableHitSound = enable;
//     }

//     // public void ChoseEasy()
//     // {
//     //     Debug.Log(easy); 
//     //     Debug.Log(easy.GetComponent<Button>().colors);

//     //     Color easyColor = easy.transform.GetComponent<Image>().color;
//     //     easyColor = Color.yellow;
//     //     easy.transform.GetComponent<Image>().color = easyColor;

//     //     Color normalColor = normal.transform.GetComponent<Image>().color;
//     //     normalColor = Color.white;
//     //     normal.transform.GetComponent<Image>().color = normalColor;

//     //     Color hardColor = hard.transform.GetComponent<Image>().color;
//     //     hardColor = Color.white;
//     //     hard.transform.GetComponent<Image>().color = hardColor;
//     // }
//     // public void ChoseNormal()
//     // {
//     //     Color easyColor = easy.transform.GetComponent<Image>().color;
//     //     easyColor = Color.white;
//     //     easy.transform.GetComponent<Image>().color = easyColor;

//     //     Color normalColor = normal.transform.GetComponent<Image>().color;
//     //     normalColor = Color.yellow;
//     //     normal.transform.GetComponent<Image>().color = normalColor;

//     //     Color hardColor = hard.transform.GetComponent<Image>().color;
//     //     hardColor = Color.white;
//     //     hard.transform.GetComponent<Image>().color = hardColor;
//     // }
//     // public void ChoseHard()
//     // {
//     //     Color easyColor = easy.transform.GetComponent<Image>().color;
//     //     easyColor = Color.white;
//     //     easy.transform.GetComponent<Image>().color = easyColor;

//     //     Color normalColor = normal.transform.GetComponent<Image>().color;
//     //     normalColor = Color.white;
//     //     normal.transform.GetComponent<Image>().color = normalColor;

//     //     Color hardColor = hard.transform.GetComponent<Image>().color;
//     //     hardColor = Color.yellow;
//     //     hard.transform.GetComponent<Image>().color = hardColor;
//     // }

//     public void DelaySlider(float value)
//     {
//         value = Mathf.Round(value * 10f) / 10f;
//         SelectedSong.delay = value;
//         delayValue.text = value.ToString();
//     }
// }