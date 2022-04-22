using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    bool keyDown = false;

    void Start()
    {
        GameManager.instance().ResetStats();
    }
    
    void Update()
    {
        if (Input.anyKey)
        {
            keyDown = true;
        }
        else if (keyDown)
        {
            keyDown = false;
            GameManager.instance().LoadFirstLevel();
        }
    }

}
