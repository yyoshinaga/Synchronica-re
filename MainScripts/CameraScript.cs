using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    // Use this for initialization
    public void Start()
    {
        // Set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        float targetAspect = 16.0f / 9.0f;

        // Determine the game window's current aspect ratio
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // Current viewport height should be scaled by this amount
        float scaleHeight = windowAspect / targetAspect;

        // Obtain camera component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // Get the screen center in normalized screen space (0.5, 0.5)
        Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f); // (0.5, 0.5) is the center of the screen in normalized coordinates
        
        // Convert screen center to world space (camera's perspective)
        Vector3 worldCenter = camera.ViewportToWorldPoint(screenCenter);

        // Set the camera's position at the world center, maintaining its current Y and Z position
        camera.transform.position = new Vector3(worldCenter.x, worldCenter.y, -10);

        // If scaled height is less than current height, add letterbox
        if (scaleHeight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            camera.rect = rect;
        }
        else // Add pillarbox
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}
