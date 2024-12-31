using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Make sure to import UnityEngine.UI for Button components.
using UnityEngine.Events;

public class Track : MonoBehaviour
{
    // Define a public variable for ID so it can be set in the Inspector
    public int buttonID;

    // Optionally, create a UnityEvent to trigger when the button is clicked
    public UnityEvent onClickEvent;

    void Start()
    {
        // Get the button component and add a listener to it.
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    // This method will be called when the button is clicked
    void OnButtonClick()
    {
        // You can use the buttonID here to identify which button was clicked.
        Debug.Log("Button with ID " + buttonID + " clicked!");

        // Call any additional events that may be associated with the button.
        onClickEvent?.Invoke();
    }
}
