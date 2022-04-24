using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public TMP_Text memoCompletedText = null;
    public TMP_Text memoDeclinedText = null;
    public TMP_Text maxPackedItemText = null;
    public TMP_Text itemRottenText = null;

    //Data Analysis
    public bool maxPackedItemCount = false;

    bool keyDown = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            keyDown = true;
        }
        else if (keyDown)
        {
            keyDown = false;
            GameManager.instance().LoadTitle();
        }

        var gameManager = GameManager.instance();
        memoCompletedText.text = string.Format("{0}: {1}", memoCompletedText.name, gameManager.memoCompletedCount);
        memoDeclinedText.text = string.Format("{0}: {1}", memoDeclinedText.name, gameManager.memoDeclinedCount);
        maxPackedItemText.text = string.Format("{0}: {1}", maxPackedItemText.name, gameManager.maxPackedItemCount);

        //record when the maxPackedItemCount
        if (!maxPackedItemCount)
        {
            Tinylytics.AnalyticsManager.LogCustomMetric("maxPackedItemCount", gameManager.maxPackedItemCount.ToString());
            maxPackedItemCount = true;
        }      

        itemRottenText.text = string.Format("{0}: {1}", itemRottenText.name, gameManager.itemRottenCount);
    }
}
