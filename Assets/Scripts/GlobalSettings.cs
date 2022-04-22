using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class GlobalSettings : SerializedScriptableObject
{
    static GlobalSettings instanceObject;
    public static GlobalSettings instance()
    {
        if (instanceObject == null)
        {
            instanceObject = Resources.Load<GlobalSettings>("GlobalSettings");
        }
        return instanceObject;
    }

    [Header("UI settings")]
    public Color invalidPlacementColor = Color.red;
    public Color validPlacementColor = Color.green;

    [Header("Rot settings")]
    [Range(1, 100)] public float rotTime;
    [Range(0, 100)] public int rotTimerAppearsAt;
    public Gradient rottenGradient = new Gradient();
    public GameObject rotTimerPrefab;
}
