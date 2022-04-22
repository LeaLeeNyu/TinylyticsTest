using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class Tag : SerializedScriptableObject, ICandidate
{
    public string tagName = "";
    public Sprite icon;

    void OnValidate()
    {
        if (tagName.Length == 0)
        {
            tagName = this.name;
        }
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public string GetName()
    {
        return tagName;
    }
}
