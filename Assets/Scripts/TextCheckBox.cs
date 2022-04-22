using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextCheckBox : MonoBehaviour, ICheckBox
{
	[SerializeField] TextMeshProUGUI label;
	[SerializeField] Image checkmark;

	[SerializeField] Color colorNormal = new Color(0f, 0f, 0f, 0f);
	[SerializeField] Color colorChecked = new Color(0.5f, 0.5f, 0.5f, 1f);

	public void Setup(string text)
	{
		label.text = text;
	}

	public void SetChecked()
	{
		label.color = colorChecked;
		checkmark.gameObject.SetActive(true);
	}

	public void SetNormal()
	{
		label.color = colorNormal;
		checkmark.gameObject.SetActive(false);
	}
}
