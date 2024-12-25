using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextHandler : MonoBehaviour 
{
    // Text objects
    public Transform earlyTextTransform;
    public Transform greatTextTransform;
    public Transform perfectTextTransform;
    public Transform slowTextTransform;
    public Transform missedTextTransform;

    private TMP_Text earlyText;
    private TMP_Text greatText;
    private TMP_Text perfectText;
    private TMP_Text slowText;
    private TMP_Text missedText;

    public void Start()
    {
        // Obtain text objects
        earlyTextTransform = transform.Find("EarlyText");
        greatTextTransform = transform.Find("GreatText");
        perfectTextTransform = transform.Find("PerfectText");
        slowTextTransform = transform.Find("SlowText");
        missedTextTransform = transform.Find("MissedText");

        // Cache TMP_Text
        earlyText = earlyTextTransform.GetComponentInChildren<TMP_Text>();
        greatText = greatTextTransform.GetComponentInChildren<TMP_Text>();
        perfectText = perfectTextTransform.GetComponentInChildren<TMP_Text>();
        slowText = slowTextTransform.GetComponentInChildren<TMP_Text>();
        missedText = missedTextTransform.GetComponentInChildren<TMP_Text>();

        // Set text opacity to 0
        SetOpacity(earlyText, 0f);
        SetOpacity(greatText, 0f);
        SetOpacity(perfectText, 0f);
        SetOpacity(slowText, 0f);
        SetOpacity(missedText, 0f);
    }

    private void SetOpacity(TMP_Text tmpText, float value)
    {
        Color color = tmpText.color;
        color.a = value;
        tmpText.color = color;
    }

    public void HandleEarlyText(Vector3 notePosition)
    {
        SetOpacity(earlyText, 1f);
        earlyTextTransform.position = notePosition;
    }

    public void HandleGreatText(Vector3 notePosition)
    {
        SetOpacity(greatText, 1f);
        greatTextTransform.position = notePosition;
    }

    public void HandlePerfectText(Vector3 notePosition)
    {
        SetOpacity(perfectText, 1f);
        perfectTextTransform.position = notePosition;
    }

    public void HandleSlowText(Vector3 notePosition)
    {
        SetOpacity(slowText, 1f);
        slowTextTransform.position = notePosition;
    }

    public void HandleMissedText(Vector3 notePosition)
    {
        SetOpacity(missedText, 1f);
        missedTextTransform.position = notePosition;
    }
}