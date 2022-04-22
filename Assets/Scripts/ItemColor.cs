using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(SpriteRenderer))]
public class ItemColor : SerializedMonoBehaviour
{
    public enum Category
    {
        Rot
    }
    [ReadOnly] public Dictionary<Category, Color> additiveColors = new Dictionary<Category, Color>();
    [ReadOnly] public Dictionary<Category, Color> colors = new Dictionary<Category, Color>();

    SpriteRenderer spriteRenderer;

    // Update is called once per frame
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetAdditiveColor(Category category, Color color)
    {
        additiveColors[category] = color;
        UpdateAdditiveColors();
    }

    public void SetColor(Category category, Color color)
    {
        colors[category] = color;
        UpdateColors();
    }

    void UpdateColors()
    {
        float r = 0;
        float g = 0;
        float b = 0;
        float a = 0;
        foreach (var color in colors)
        {
            r += color.Value.r;
            g += color.Value.g;
            b += color.Value.b;
            a += color.Value.a;
        }
        if (colors.Count > 0)
        {
            r /= colors.Count;
            g /= colors.Count;
            b /= colors.Count;
            a /= colors.Count;
            spriteRenderer.material.SetColor("_Colorize", new Color(r, g, b, a));
        }
    }

    void UpdateAdditiveColors()
    {
        float r = 0;
        float g = 0;
        float b = 0;
        float a = 0;
        foreach (var color in colors)
        {
            r += color.Value.r;
            g += color.Value.g;
            b += color.Value.b;
            a += color.Value.a;
        }
        if (a > 0)
        {
            r /= a;
            g /= a;
            b /= a;
            spriteRenderer.material.SetColor("_AdditiveColor", new Color(r, g, b, 1));
        }
    }
}
