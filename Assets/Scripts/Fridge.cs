using System.Collections.Generic;
using UnityEngine;

public static class Fridge
{
    public static GameObject Get()
    {
        var fridge = GameObject.FindGameObjectWithTag("Fridge");
        if (fridge == null)
        {
            Debug.LogError("Unable to find fridge. The fridge object must be tagged 'Fridge'.");
        }
        return fridge;
    }
}