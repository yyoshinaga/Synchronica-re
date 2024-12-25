using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

using HitStatus = ScoreManager.HitStatus;  // Alias GameManager.HitStatus as HitStatus
using Rank = SelectedSong.Rank;  // Alias SelectedSong.Rank as Rank

public class ScoreAnimation : MonoBehaviour
{
    // UI Elements
    [Header("UI Elements")]
    public GameObject scoreBackground;
    public GameObject trackClearedText;
    public GameObject trackFailedText;
    public GameObject scoreDisplay;
    public GameObject perfectScoreDisplay;
    public GameObject greatScoreDisplay;
    public GameObject slowScoreDisplay;
    public GameObject earlyScoreDisplay;
    public GameObject missedScoreDisplay;
    public GameObject rankDisplay;
    public GameObject buttonObjects;
    public GameObject scoreObjects;
    public Canvas canvas; 

    // Internal Variables
    private Color scoreBackgroundColor;
    private Vector2 scoreBackgroundScale;

    private Color trackClearedTextColor;
    private Color trackFailedTextColor;

    private GameObject displayText;
    private Vector2 textScale;
    private Color textColor;

    private bool isOpeningComplete;
    private bool isScoreAnimationComplete;
    private bool isDoingOpening;
    private bool isDoingScoreAnimation;

    // Start is called before the first frame update
    void Start()
    {
        InitializeVariables();
        
        Debugging();

        // Determine if cleared or failed
        SetDisplayText();

        scoreDisplay.GetComponent<TMP_Text>().text = (GameScore.score).ToString();
    }

    void Debugging()
    {
        Dictionary<Rank, int> ScoreDict = new Dictionary<Rank, int>();

        ScoreDict.Add(Rank.S, 2000);
        ScoreDict.Add(Rank.A, 1800);
        ScoreDict.Add(Rank.B, 1400);
        ScoreDict.Add(Rank.C, 1000);
        ScoreDict.Add(Rank.D, 800);
        ScoreDict.Add(Rank.E, 600);
        ScoreDict.Add(Rank.F, 500);

        SelectedSong.SetRank(ScoreDict);

        ScoreManager.AddScore(HitStatus.EARLY);
        ScoreManager.AddScore(HitStatus.GREAT);
        ScoreManager.AddScore(HitStatus.PERFECT);
        ScoreManager.AddScore(HitStatus.SLOW);
        ScoreManager.AddScore(HitStatus.MISSED);
    }

    // Initialize variables to default values
    private void InitializeVariables()
    {
        isOpeningComplete = false;
        isScoreAnimationComplete = false;
        isDoingOpening = false;
        isDoingScoreAnimation = false;

        // Disable 
        scoreObjects.SetActive(false); // All text
        buttonObjects.SetActive(false);
    }

    // Set which text to display (track cleared or track failed)
    private void SetDisplayText()
    {   
        int score = ScoreManager.GetScore();
        if (SelectedSong.GetRank(score) != Rank.F)
        {
            displayText = trackClearedText;
        }
        else
        {
            displayText = trackFailedText;
        }
        
        // Make invisible
        textColor = displayText.GetComponent<TMP_Text>().color;
        textColor.a = 0;
        displayText.GetComponent<TMP_Text>().color = textColor;

        // Center text on canvas
        displayText.transform.position = canvas.transform.position;
    }

    void Update()
    {
        // Make sure update doesn't call the coroutine more than once
        if (!isOpeningComplete && !isDoingOpening)
        {   
            isDoingOpening = true;
            StartCoroutine(AnimateGameStatus());
        }
        else if (isOpeningComplete && !isScoreAnimationComplete && !isDoingScoreAnimation) 
        {
            isDoingScoreAnimation = true;
            StartCoroutine(UpdateScores());
        }
    }

    // Coroutine to update score displays
    private IEnumerator UpdateScores()
    {   
        // Enable 
        scoreObjects.SetActive(true);
        buttonObjects.SetActive(true);

        // Make rankDisplay transparent
        Color rankColor = rankDisplay.GetComponent<TMP_Text>().color;
        rankColor.a = 0f;
        rankDisplay.GetComponent<TMP_Text>().color = rankColor;
        
        // Wait
        yield return new WaitForSeconds(2);

        // Perform updating score animation
        yield return UpdateScoreDisplay(HitStatus.EARLY, earlyScoreDisplay);
        yield return UpdateScoreDisplay(HitStatus.GREAT, greatScoreDisplay);
        yield return UpdateScoreDisplay(HitStatus.PERFECT, perfectScoreDisplay);
        yield return UpdateScoreDisplay(HitStatus.SLOW, slowScoreDisplay);
        yield return UpdateScoreDisplay(HitStatus.MISSED, missedScoreDisplay);

        // Wait
        yield return new WaitForSeconds(2);

        // Perform animation on rank display
        yield return UpdateRankDisplay();

        // Mark as finished
        isScoreAnimationComplete = true;
    }

    // Helper method to update individual score display
    private IEnumerator UpdateScoreDisplay(HitStatus hitStatus, GameObject scoreDisplay)
    {   
        // This function counts up the score, updating the value in the
        // gui one point at a time
        int totalPoints = ScoreManager.CalcHitStatusPoints(hitStatus);
        for (int count = 0; count <= totalPoints; count++)
        {
            scoreDisplay.GetComponent<TMP_Text>().text = count.ToString();
            yield return new WaitForSeconds(0.002f);
        }
    }

    private IEnumerator UpdateRankDisplay()
    {
        // Get components
        Vector3 rankInitPosition = rankDisplay.transform.position;
        Vector2 rankScale = rankDisplay.transform.localScale;
        Color rankColor = rankDisplay.GetComponent<TMP_Text>().color;

        // Make rankDisplay transparent
        rankColor.a = 0f;
        rankDisplay.GetComponent<TMP_Text>().color = rankColor;

        // Move position to center of canvas
        rankDisplay.transform.position = canvas.transform.position;

        // Make scale really large at first
        rankScale.x = 500;
        rankScale.y = 500;
        rankDisplay.transform.localScale = rankScale;

        // Set text
        rankDisplay.GetComponent<TMP_Text>().text = 
            SelectedSong.RankToString(SelectedSong.GetRank(GameScore.score));

        int steps = 100;
        float finalScale = 1f;
        float initScale = rankScale.x;
        float t = 0f;
        float scale = 0f;

        for (int i = 0; i <= steps; i++)
        {
            // Scale down and decelerate
            // (final-init) * (i/step)^0.5 + init
            t = (float)i / steps;
            scale = (finalScale - initScale) * Mathf.Pow(t, 0.5f) + initScale;
            rankScale.x = scale;
            rankScale.y = scale;
            rankDisplay.transform.localScale = rankScale;
            
            // Change opacity of rankDisplay
            rankColor.a += 1f / steps;
            rankDisplay.GetComponent<TMP_Text>().color = rankColor;

            // Change position of rankDisplay
            // (final-init) * (i/step) + init
            rankDisplay.transform.position = 
                (rankInitPosition - canvas.transform.position) * (float)i / steps 
                    + canvas.transform.position;
            yield return new WaitForSeconds(0.01f);
        } 
    }

    // Coroutine to handle text fade-in, fade-out, and scaling effects
    private IEnumerator AnimateGameStatus()
    {   
        yield return new WaitForSeconds(2); // Initial wait before fading in
        Vector3 initScale = displayText.transform.localScale;
        Vector3 finalScale = new Vector3(1,1,1);

        int steps = 10;
        Vector3 nextScale = new Vector3(0,0,0);

        // Fade in and scale up the text
        for (int i = 0; i <= steps; i++)
        {
            nextScale = (finalScale - initScale) * (float)i/steps + initScale;
            displayText.transform.localScale = nextScale;

            textColor.a = (float)i/steps;
            displayText.GetComponent<TMP_Text>().color = textColor;
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(2); // Wait before fading out

        // Fade out text
        for (int i = 0; i <= steps; i++)
        {
            textColor.a = 1f - (float)i/steps;
            displayText.GetComponent<TMP_Text>().color = textColor;
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(1); // Wait before finishing

        isOpeningComplete = true;
    }
}
