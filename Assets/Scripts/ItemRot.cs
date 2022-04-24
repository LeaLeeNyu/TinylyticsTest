using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(ItemColor))]
public class ItemRot : MonoBehaviour
{
    public bool rotting = true;
    [Range(0, 1)] public float rotProgress = 0;
    [Tooltip("Readonly")] public float remainingTime;

    private float left;
    private float right;
    private float bottom;
    private float top;

    private bool trashing = false;

    //Data Analysis
    public bool itemDragInFridge = false;
    public bool itemTrashing = false;

    ItemColor itemColor;
    public GameObject countDown = null;

    void Awake()
    {
        itemColor = GetComponent<ItemColor>();
    }

    private void Start()
    {
        var fridge = Fridge.Get();
        left = fridge.transform.Find("Left").position.x;
        right = fridge.transform.Find("Right").position.x;
        bottom = fridge.transform.Find("Bottom").position.y;
        top = fridge.transform.Find("Top").position.y;
    }

    void Update()
    {
		if (!trashing && !IsInsideFridge() && rotProgress == 1)
        {
            this.trashing = true;
            GameManager.instance().IncreaseItemRotten();
        }
		if (trashing)
        {
            //record when player drag the item into fridge 
            if (!itemTrashing)
            {
                Tinylytics.AnalyticsManager.LogCustomMetric("itemTrashing", gameObject.GetComponent<Item>().itemName.ToString());
                itemTrashing = true;
            }
           
            this.transform.localScale += new Vector3(-0.01f, -0.01f, -0.01f);
            this.transform.Rotate(0, 0, 3);
        }
        if (this.transform.localScale.x < 0.1f)
        {
			GameObject.Destroy(this.gameObject);
            return;
        }
        if (IsInsideFridge())
        {
            rotting = false;        
        }
        else
        {
            rotting = true;
        }
        if (rotting)
        {
            var setting = GlobalSettings.instance();
            rotProgress = Mathf.Clamp01(rotProgress + Time.deltaTime /  setting.rotTime);
            itemColor.SetColor(ItemColor.Category.Rot, setting.rottenGradient.Evaluate(rotProgress));
            if (countDown == null)
            {
                countDown = GameObject.Instantiate<GameObject>(setting.rotTimerPrefab, this.transform, false);
            }
            remainingTime = setting.rotTime * (1 - rotProgress);
            if (remainingTime <= setting.rotTimerAppearsAt)
            {
                countDown.SetActive(true);
                countDown.GetComponent<TMP_Text>().text = Mathf.FloorToInt(remainingTime).ToString();
            }
            else
            {
                countDown.SetActive(false);
            }
        }
        else
        {
            if (countDown != null)
            {
                countDown.SetActive(false);   
            }
        }
    }

    public bool IsInsideFridge()
    {
        return this.transform.position.x > left
            && this.transform.position.x < right
            && this.transform.position.y > bottom
            && this.transform.position.y < top;
    }

    //Data Analysis
    public void ItemDragInFridge()
    {
        if (!itemDragInFridge)
        {
            Tinylytics.AnalyticsManager.LogCustomMetric("itemDragInFridge", gameObject.GetComponent<Item>().itemName.ToString());
            itemDragInFridge = true;
        }      
    }

}
