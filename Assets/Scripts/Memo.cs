using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Memo : MonoBehaviour
{
	public Recipe recipe;
	public float yThreshold = 23.6f;

	[SerializeField] Transform iconLayout;
	[SerializeField] Transform textLayout;
	[SerializeField] ImageCheckBox memoIconPrefab;
	[SerializeField] TextCheckBox memoTextPrefab;

	private List<ICandidate> ingredients = new List<ICandidate>();
	private List<ICheckBox> checkBoxes = new List<ICheckBox>();
	public List<bool> ingredientStates = new List<bool>();
	private Vector2 clickPos;
	private Vector2 startPos;
	private Image image;

	private void Awake() {
    image = GetComponent<Image>();
	}

  private void Start()
	{
		StartCoroutine(DelayRegisterTransform());
	}

	public void Setup(Recipe recipe)
	{
		ingredients.Clear();
		checkBoxes.Clear();
		ingredientStates.Clear();
		foreach(Transform child in iconLayout)
		{
			Destroy(child.gameObject);
		}

		foreach(Transform child in textLayout)
		{
			Destroy(child.gameObject);
		}

		this.recipe = recipe;

		if (recipe.GetRecipeType() == RecipeType.TEXT)
		{
			foreach (var candidate in recipe.candidates)
			{
				ingredients.Add(candidate);
				ingredientStates.Add(false);
				TextCheckBox textCheckBox = Instantiate(memoTextPrefab, textLayout).GetComponent<TextCheckBox>();
				textCheckBox.Setup(candidate.GetName());
				textCheckBox.SetNormal();
				checkBoxes.Add(textCheckBox);
			}
		}
		else if (recipe.GetRecipeType() == RecipeType.IMAGE)
		{
			foreach (var candidate in recipe.candidates)
			{
				ingredients.Add(candidate);
				ingredientStates.Add(false);
				ImageCheckBox imageCheckBox = Instantiate(memoIconPrefab, iconLayout).GetComponent<ImageCheckBox>();
				imageCheckBox.Setup(candidate.GetIcon());
				imageCheckBox.SetNormal();
				checkBoxes.Add(imageCheckBox);
			}
		}
	}

	public bool CheckItem(Item item)
	{
		if (recipe == null)
			return false;

		for (int i = 0; i < ingredients.Count; i++)
		{
			if (ingredientStates[i] == true) continue;
			if (item.Matches(ingredients[i]))
			{
				Debug.Log("Checked goal: " + ingredients[i].GetName());
				ingredientStates[i] = true;
				checkBoxes[i].SetChecked();
				CheckMenuComplete();
				return true;
			}
		}
		return false;
	}

	public bool IsPotentialItem(Item item)
	{
		if (recipe == null)
			return false;

		for (int i = 0; i < ingredients.Count; i++)
		{
			if (ingredientStates[i] == true) continue;
			if (item.Matches(ingredients[i]))
			{
				return true;
			}
		}
		return false;
	}

	private void CheckMenuComplete()
	{
		if (ingredientStates == null || ingredientStates.Count == 0) return;
		foreach (bool value in ingredientStates)
		{
			if (!value) return;
		}
		ItemManager.Instance.UpdateMemo(this, false);
		GameManager.instance().IncreaseMemoCompleted();
		Debug.Log("recipe complete");
	}

    private void OnMouseDown()
    {
		clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

    private void OnMouseDrag()
    {
		Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		this.transform.position = startPos + new Vector2(mousePos.x, mousePos.y) - clickPos;
    }

    private void OnMouseUp()
	{
		if (this.transform.position.y > yThreshold)
		{
			this.Decline();
		} else
        {
			this.transform.position = startPos;
		}
	}

	private void Decline()
	{
    GameManager.instance().IncreaseMemoDeclined();
		ItemManager.Instance.UpdateMemo(this, true);
		this.transform.position = startPos;
	}

	private IEnumerator DelayRegisterTransform()
	{
		yield return new WaitForSeconds(0.5f);
		startPos = this.transform.position;
	}

  void OnTriggerEnter2D(Collider2D collider)
	{
		var dragAndSnap = collider.GetComponent<DragAndSnap>();
		if (dragAndSnap != null && dragAndSnap.isDragging) {
			if (IsPotentialItem(collider.GetComponent<Item>())) {
				image.color = GlobalSettings.instance().validPlacementColor;
			} else {
	   		image.color = GlobalSettings.instance().invalidPlacementColor; 
			}
		}
	}

	void OnTriggerExit2D(Collider2D collider)
	{
		image.color = Color.white;
	}
}
