using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Gesture : SerializedMonoBehaviour
{
    public static Gesture instance;

    bool rightFlingFrame = false;
    float beenToLeftAt = -1;
    [Range(0, 0.5f)] public float flingDetectWidthRatio = 0.3f;
    [Range(0, 1)] public float maxflingtTime = 0.1f;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        ResetFlingFrames();
        DetectFlingFrame();
    }

    public int GetHorizontalFling()
    {
        if (rightFlingFrame)
        {
            return 1;
        }
        return 0;
    }

    void ResetFlingFrames()
    {
        if (rightFlingFrame)
        {
            rightFlingFrame = false;
        }
    }


    void DetectFlingFrame()
    {
        if (Input.GetMouseButton(0))
        {
            if (Input.mousePosition.x < Screen.width * flingDetectWidthRatio)
            {
                beenToLeftAt = Time.time;
            }
            if (Input.mousePosition.x > Screen.width * (1 - flingDetectWidthRatio) &&
            Time.time - beenToLeftAt <= maxflingtTime)
            {
                beenToLeftAt = -1;
                rightFlingFrame = true;
            }
        }
    }
}
