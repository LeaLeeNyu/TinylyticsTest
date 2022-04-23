using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Gesture))]
public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance
    {
		get
        { 
			var instance = Object.FindObjectOfType<ItemManager>();
			if (instance == null)
			{
				Debug.LogError("Cannot find ItemManager in the current scene.");
			}
			return instance;
        }

    }

    public GameObject prototypes;
    public GameObject container;
    public float secondsPerUpdate;
    public LevelData levelData;
	public TextMeshProUGUI winText;
	public List<Memo> memos = new List<Memo>();
    public List<Recipe> recipePool = new List<Recipe>();

    private List<GameObject> prototypeObjects = new List<GameObject>();
    private float time = 0.0f;
    private int index = 0;
    Dictionary<Tag, HashSet<Item>> tagToItems = new Dictionary<Tag, HashSet<Item>>();

    void Awake()
    {
        ValidateLevelData();
        SetupRecipePool();
        SetupTagToItems();
    }

    void SetupTagToItems() {
        tagToItems.Clear();
        var items = Resources.LoadAll<Item>("Items");
        foreach (var item in items)
        {
            foreach (var tag in item.tags) {
                if (!tagToItems.ContainsKey(tag)) {
                    tagToItems[tag] = new HashSet<Item>();
                }
                tagToItems[tag].Add(item);
            }
        }
    }

	private void Start()
	{
		this.SpawnRecipe();
        winText.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            GameManager.instance().LoadNextLevel();
        }

        time += Time.deltaTime;
        if (time >= secondsPerUpdate) {
            time = 0.0f;
            this.SpawnItem();
        } else if (Input.GetKeyUp(KeyCode.DownArrow)) {
            this.SpawnItem();
        }
    }

	#region public methods
	public void UpdateMemo(Memo memo, bool declined)
	{
        if (declined) {
            this.recipePool.Add(memo.recipe);
        }
		Recipe recipe = null;
		if (GetNextRecipe(out recipe))
		{
            memo.Setup(recipe);
			memo.gameObject.SetActive(true);
			Debug.LogFormat("Spawned recipe {0}", recipe);
		}
		else
		{
            memos.Remove(memo);
			Destroy(memo.gameObject);
			if (memos.Count == 0)
            {
                GameManager.instance().LoadNextLevel();
			}
		}
	}

    public void UpdateMaxPackedItem() {
        var itemRots = GetComponentsInChildren<ItemRot>();
        int count = 0;
        foreach (var itemRot in itemRots) {
            if (itemRot.IsInsideFridge()) {
                count++;
            }
        }
        GameManager.instance().UpdateMaxPackedItem(count);
    }

	#endregion


	void ValidateLevelData()
    {
        if (levelData == null)
        {
            Debug.LogErrorFormat("{0} in scene {1} does not have any level data.", this.GetType().Name, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }

    private void SpawnItem()
    {
        Item item = null;
        if (GetNextItem(out item))
        {
            GameObject newObject = GameObject.Instantiate<GameObject>(item.gameObject, container.transform);
            newObject.transform.position = this.transform.position;
            newObject.name = "item-" + this.index;
            this.index++;
            foreach (Transform t in newObject.transform.GetComponentsInChildren<Transform>())
            {
                t.transform.GetComponent<PolygonCollider2D>().enabled = false;
                if (t.tag == "Icon")
                {
                    var sprite = t.gameObject.GetComponent<SpriteRenderer>().sprite;
                    var points = t.gameObject.GetComponent<PolygonCollider2D>().points;
                    t.gameObject.GetComponent<SpriteRenderer>().sprite = newObject.transform.GetComponent<SpriteRenderer>().sprite;
                    t.transform.GetComponent<PolygonCollider2D>().points = newObject.transform.GetComponent<PolygonCollider2D>().points;
                    newObject.transform.GetComponent<SpriteRenderer>().sprite = sprite;
                    newObject.transform.GetComponent<PolygonCollider2D>().points = points;
                    newObject.transform.GetComponent<PolygonCollider2D>().enabled = true;
                }
            }
            newObject.SetActive(true);

            //record when instantiate new item
            Tinylytics.AnalyticsManager.LogCustomMetric("itemLoadInScene", newObject.GetComponent<Item>().itemName.ToString());
        }
        else
        {
            Debug.Log("No more item to spawn.");
        }
    }

    private void SpawnRecipe()
    {
		for (int i = 0; i < memos.Count; i++)
		{
			Recipe recipe = null;
			if (GetNextRecipe(out recipe))
			{
				memos[i].Setup(recipe);
				memos[i].gameObject.SetActive(true);
				Debug.LogFormat("Spawned recipe {0}", recipe);
			}
		}
    }

    bool GetNextItem(out Item item)
    {
        var items = GetItemPool();
        if (items.Count == 0) {
            item = null;
            return false;
        } else {
            item = items[Random.Range(0, items.Count)];
        }
        return true;
    }

    // Gets potential items based on recipes in memo and pool, excluding the ones already spawned.
    List<Item> GetItemPool() {
        HashSet<Item> set = GetMemoRecipeItems();
        set.UnionWith(GetRecipePoolItems());
        HashSet<string> spawnedItemNames = GetSpawnedItemNames();
        set.RemoveWhere(item =>
        {
            return spawnedItemNames.Contains(item.itemName);
        });
        return new List<Item>(set);
    }

    HashSet<Item> GetMemoRecipeItems()
    {
        HashSet<Item> items = new HashSet<Item>();
        foreach (var memo in memos)
        {
            for (int i = 0; i < memo.ingredientStates.Count; i++)
            {
                if (!memo.ingredientStates[i]) {
                    ICandidate candidate = memo.recipe.candidates[i];
                    if (candidate.GetType() == typeof(Tag))
                    {
                        foreach (var tagItem in tagToItems[(Tag)candidate])
                        {
                            items.Add(tagItem);
                        }
                    }
                    else
                    {
                        items.Add((Item)candidate);
                    }
                }
            }
        }
        return items;
    }

    HashSet<Item> GetRecipePoolItems()
    {
        HashSet<Item> items = new HashSet<Item>();
        foreach (var recipe in recipePool) {
            foreach (var candidate in recipe.candidates)
            {
                if (candidate.GetType() == typeof(Tag))
                {
                    foreach (var tagItem in tagToItems[(Tag)candidate])
                    {
                        items.Add(tagItem);
                    }
                }
                else
                {
                    items.Add((Item)candidate);
                }
            }
        }
        return items;
    }

    [Button] HashSet<string> GetSpawnedItemNames() {
        var items = GetComponentsInChildren<Item>();
        HashSet<string> names = new HashSet<string>();
        foreach (var item in items)
        {
            names.Add(item.itemName);
        }
        return names;
    }

    [Button]
    void SetupRecipePool()
    {
        recipePool.Clear();
        foreach (var recipe in levelData.spawnedRecipes)
        {
            for (int i = 0; i < recipe.count; i++)
            {
                recipePool.Add(recipe.recipe);
            }
        }
        Shuffle.Do<Recipe>(recipePool);
    }

    bool GetNextRecipe(out Recipe recipe)
    {
        if (recipePool.Count == 0)
        {
            recipe = null;
            return false;
        }
        int index = Random.Range(0, recipePool.Count);
        recipe = recipePool[index];
        recipePool.RemoveAt(index);
        return true;
    }

    [Button]
    void GetNextRecipeTest()
    {
        Recipe recipe = null;
        bool result = GetNextRecipe(out recipe);
        Debug.LogFormat("GetNextRecipe() {0} {1}", result, recipe);
    }
}
