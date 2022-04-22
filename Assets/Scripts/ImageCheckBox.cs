using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCheckBox : MonoBehaviour, ICheckBox
{
	[SerializeField] Image image;
	[SerializeField] Image checkMark;

	[SerializeField] Color colorNormal = new Color(1f, 1f, 1f, 1f);
	[SerializeField] Color colorChecked = new Color(0, 0, 0, 0.5f);

	public void Setup(Sprite icon)
	{
		image.sprite = icon;
	}

	public void SetChecked()
	{
		image.color = colorChecked;
		checkMark.gameObject.SetActive(true);
	}

	public void SetNormal()
	{
		image.color = colorNormal;
		checkMark.gameObject.SetActive(false);
	}
}
