using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class LevelData : SerializedScriptableObject
{
    [System.Serializable]
    public class ItemCount
    {

        [PreviewField][AssetSelector(Paths = "Assets/Prefabs/Items")] public GameObject item;
        [Range(1, 100)] public int count;
    }

    [System.Serializable]
    public class RecipeCount
    {

        public Recipe recipe;
        [Range(1, 100)] public int count;
    }

    [TableList]
    public List<RecipeCount> spawnedRecipes = new List<RecipeCount>();
}
