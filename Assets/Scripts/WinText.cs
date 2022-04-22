using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    void Update()
    {
        text.color = Color.HSVToRGB(Mathf.PingPong(Time.time * .15f, 1), 0.9f, 1f);
    }
}
