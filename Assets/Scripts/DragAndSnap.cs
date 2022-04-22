using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class DragAndSnap : MonoBehaviour
{

    [Tooltip("[Readonly] count of overllaping colliders.")]
    public int overlappingSmallColliderCount = 0;

    public bool isDragging;

    private ItemManager itemManager;
    private float left;
    private float right;
    private float bottom;
    private float top;

    private HashSet<Collider2D> overlappingSmallColliders = new HashSet<Collider2D>();
    private HashSet<Collider2D> overlappingColliders = new HashSet<Collider2D>();
    private List<Collider2D> colliders = new List<Collider2D>();
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool clicked;

    private GameObject placeholder = null;
    private ItemRot itemRot;
    private Vector2 clickPos;

    void Awake()
    {
        GetComponentsInChildren<Collider2D>(colliders);
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        itemRot = this.gameObject.GetComponent<ItemRot>();
    }

    private void Start()
    {
        itemManager = ItemManager.Instance;

        var fridge = Fridge.Get();
        left = fridge.transform.Find("Left").position.x;
        right = fridge.transform.Find("Right").position.x;
        bottom = fridge.transform.Find("Bottom").position.y;
        top = fridge.transform.Find("Top").position.y;
    }

    void OnMouseDown()
    {
        clickPos = this.transform.position;
        if (itemRot.rotProgress == 1)
        {
            return;
        }
        CreatePlaceHolder();

        if (!clicked)
        {
            foreach (Transform t in this.transform.GetComponentsInChildren<Transform>())
            {
                if (t.tag == "Icon")
                {
                    var sprite = t.gameObject.GetComponent<SpriteRenderer>().sprite;
                    var points = t.gameObject.GetComponent<PolygonCollider2D>().points;
                    this.transform.GetComponent<SpriteRenderer>().sprite = sprite;
                    this.transform.GetComponent<PolygonCollider2D>().points = points;
                }
                if (t.tag == "SmallerBound")
                {
                    t.transform.GetComponent<PolygonCollider2D>().enabled = true;
                }
            }
        }
        this.placeholder.SetActive(true);
        clicked = true;
        isDragging = true;
        foreach (var collider in colliders)
        {
            collider.isTrigger = true;
        }
    }

    void CreatePlaceHolder()
    {
        this.placeholder = GameObject.Instantiate<GameObject>(this.gameObject, this.transform.parent.transform);
        var placeHolderPosition = this.placeholder.transform.position;
        placeHolderPosition.z += 0.01f;
        this.placeholder.transform.position = placeHolderPosition;
    }

    void OnMouseUp()
    {
        isDragging = false;
        if (!IsInsideFridge(this.transform.position.x, this.transform.position.y) && !IsInsideFridge(clickPos.x, clickPos.y))
        {
            Rotate(this.placeholder, 90);
        }
        foreach (var collider in colliders)
        {
            collider.isTrigger = false;
        }
        if (IsOverlapping())
        {
            foreach (var collider in overlappingColliders)
            {
                if (collider.GetComponent<Memo>() != null)
                {
                    Memo memo = collider.GetComponent<Memo>();
                    bool success = memo.CheckItem(this.GetComponent<Item>());
                    if (success)
                    {
                        Destroy(this.gameObject);
                        Destroy(placeholder);
                        return;
                    }
                }
            }
        }

        if (IsSmallOverlapping())
        {
            Destroy(this.gameObject);
        }
        else
        {
            SetValidPlacement();
            Destroy(this.placeholder);
            this.placeholder = null;
            ItemManager.Instance.UpdateMaxPackedItem();
        }

        overlappingColliders.Clear();
        overlappingSmallColliders.Clear();
    }

    void Update()
    {
        if (isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector2((float)System.Convert.ToInt32(mousePos.x), (float)System.Convert.ToInt32(mousePos.y));

            int flingDirection = Gesture.instance.GetHorizontalFling();
            if (flingDirection != 0)
            {
                Rotate(this.gameObject, -90 * flingDirection);
            }
        }

        overlappingSmallColliderCount = overlappingSmallColliders.Count;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        overlappingColliders.Add(collider);
        if (IsCollidingWithSmallBound(collider))
        {
            overlappingSmallColliders.Add(collider);
            SetInvalidPlacement();
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        overlappingColliders.Remove(collider);
        overlappingSmallColliders.Remove(collider);
        if (!IsSmallOverlapping())
        {
            SetValidPlacement();
        }
    }

    bool IsCollidingWithSmallBound(Collider2D collider)
    {
        if (collider.gameObject.tag != "SmallerBound")
        {
            return false;
        }
        var colliderParent = collider.transform.parent;
        if (colliderParent == null)
        {
            return true;
        }
        if (colliderParent.gameObject == this.placeholder)
        {
            return false;
        }
        return true;
    }

    bool IsSmallOverlapping()
    {
        return overlappingSmallColliders.Count > 0;
    }

    bool IsOverlapping()
    {
        return overlappingColliders.Count > 0;
    }

    void SetInvalidPlacement()
    {
        if (isDragging)
        {
            spriteRenderer.color = GlobalSettings.instance().invalidPlacementColor;
        }
    }

    void SetValidPlacement()
    {
        spriteRenderer.color = originalColor;
    }

    bool IsInsideFridge(float x, float y)
    {
        return x > left && x < right && y > bottom && y < top;
    }

    void Rotate(GameObject gameObject, float degrees)
    {
        gameObject.transform.Rotate(new Vector3(0, 0, degrees));
        TMP_Text text = gameObject.GetComponent<ItemRot>().countDown.GetComponent<TMP_Text>();
        if (text != null)
        {
            text.transform.Rotate(new Vector3(0, 0, -degrees));
        }
    }
}
