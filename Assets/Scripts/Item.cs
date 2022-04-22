using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemRot), typeof(DragAndSnap))]
public class Item : MonoBehaviour, ICandidate
{
    public string itemName = "";
    public List<Tag> tags = new List<Tag>();

    [SerializeField] Sprite icon;

    void OnValidate()
    {
        if (itemName.Length == 0)
        {
            itemName = this.name;
        }
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public string GetName()
    {
        return itemName;
    }

    public bool Matches(ICandidate target)
    {
        if (target.GetType() == typeof(Item))
        {
            return GetName() == target.GetName();
        }
        else if (target.GetType() == typeof(Tag))
        {
			Object targetObject = (Object)target;
			foreach (Tag tag in tags)
            {
                if (tag == targetObject) return true;
            }
        }
        return false;
    }

    public override string ToString()
    {
        return GetName();
    }
}
