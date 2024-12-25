using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoteAnimation
{
    // Projectile
    public const float prjScaleUpAmount = 0.004f; //0.005f; //0.025f; //0.005
    public const float prjScaleDownSlowAmount = -0.0025f; //0.025f; //0.004
    public const float prjScaleDownFastAmount = -0.005f; //0.05f; //0.005
    public const float prjDisappearAmount = -0.1f;
    public const float prjAppearAmount = 0.01f; // May need to tune

    // Target
    public const float targetDistanceOffset = 10;
    public const float targetScaleUpAmount = 0.005f;
    public const float targetScaleUpSlightAmount = 0.001f;    
    public const float targetAppearAmount = 0.005f;
    public const float targetDisappearAmount = -0.1f;
    public const float targetScaleDownSlightAmount = -0.05f;
    public const float targetMaxScale = 2f;
    public const float targetInitScale = 0.02f;

    // Ring variables
    public const float ringScaleUpAmount = 0.025f;
    public const float ringAppearAmount = 0.2f;
    public const float ringDisappearAmount = -0.015f;
    public const float ringMaxScale = 2.25f;

    // Link variables
    public const float linkAppearAmount = 0.01f;

    public static void SetOpacityToZero(SpriteRenderer sprite)
    {
        Color color = sprite.color;
        color.a = 0.0f;                 
        sprite.color = color;
    }

    public static void ChangeScale(Transform transform, float amount)
    {
        Vector3 scale = transform.localScale;
        if (amount > 0)
        {
            // No cap necessary
            scale.x += amount;
            scale.y += amount;
        }
        else
        {
            // Amount is negative so adding is okay
            scale.x = Mathf.Max(scale.x + amount, 0); // Cap at 0
            scale.y = Mathf.Max(scale.y + amount, 0); // Cap at 0
        }

        // Debug.Log($"before transform position: {transform.position}");
        // Apply change
        transform.localScale = scale;
        // Debug.Log($"after transform position: {transform.position}");

    }

    public static void ChangeOpacity(SpriteRenderer sprite, float amount)
    {
        Color color = sprite.color;
        if (amount > 0)
        {
            color.a = Mathf.Min(color.a + amount, 1); // Cap at 1
        }
        else 
        {
            // Amount is negative so adding is okay
            color.a = Mathf.Max(color.a + amount, 0); // Cap at 0
        }
        // Must apply the color to sprite
        sprite.color = color;
    }

    public static void SetLineOpacityToZero(LineRenderer line)
    {
        // Get the current start and end color
        Color startColor = line.startColor;
        Color endColor = line.endColor;
        
        // Set the new start and end color with the desired alpha value
        line.startColor = 
            new Color(startColor.r, startColor.g, startColor.b, 0);
        line.endColor = 
            new Color(endColor.r, endColor.g, endColor.b, 0);
    }

    public static void ChangeLineOpacity(LineRenderer line, float amount)
    {
        // Get the current start and end color
        Color startColor = line.startColor;
        Color endColor = line.endColor;

        float alpha = 0f;
        if (amount > 0)
        {
            alpha = Mathf.Min(startColor.a + amount, 1);
        }
        else
        {
            alpha = Mathf.Max(startColor.a + amount, 0);
        }

        // Set the new start and end color with the desired alpha value
        line.startColor = 
            new Color(startColor.r, startColor.g, startColor.b, alpha);
        line.endColor = 
            new Color(endColor.r, endColor.g, endColor.b, alpha);
    }

    public static bool IsMaxScale(Transform transform, float maxValue)
    {
        return transform.localScale.x >= maxValue && 
                transform.localScale.y >= maxValue;
    }

    public static void SetScale(Transform transform, float value)
    {
        transform.localScale = new Vector3(value, value, transform.localScale.z);
    }
}