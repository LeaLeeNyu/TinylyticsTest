using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum RecipeType
{
	NONE,
	IMAGE,
	TEXT
}

[CreateAssetMenu]
public class Recipe : SerializedScriptableObject
{
    [AssetSelector(Paths = "Assets/Prefabs/Items|Assets/Tags")]
    public List<ICandidate> candidates = new List<ICandidate>();
    public int score;

	public RecipeType GetRecipeType()
    {
		if (candidates == null || candidates.Count == 0)
        {
            return RecipeType.NONE;
		}

		foreach (ICandidate candidate in candidates)
        {
			if (candidate.GetType() == typeof(Tag))
            {
                return RecipeType.TEXT;
			}
		}

        return RecipeType.IMAGE;
	}

    public override string ToString()
    {
        List<string> candidateNames = new List<string>();
        foreach (var candidate in candidates)
        {
            candidateNames.Add(candidate.GetName());
        }
        return string.Format("<Recipe> score = {0} {1}", score, string.Join(",", candidateNames));
    }
}
