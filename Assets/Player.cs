using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
public class Player : MonoBehaviour
{
    public enum MenuState { NONE, INVENTORY, SHOP }
    public MenuState menuState;
    public PlayerController controls;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject _selector;
    [SerializeField] GameObject _bound1;
    [SerializeField] GameObject _bound2;
    [SerializeField] GameObject _tileSelector;
    [SerializeField] ItemBarSelector _itemSelector;
    [SerializeField] ItemBarSelector _inventorySelector;
    [SerializeField] SpriteRenderer spriteRenderer;
    Item _selectedItem;
    [SerializeField] float _speed;
    float _acceleration = 10;
    float _deceleration = 15;
    float _velPower = 0.9f;
    public Vector2 move;
    public Vector2 aim;
    Vector2 _look;
    public bool paused;
    public bool canMove;

    [Header("UI stuff")]
    public GameObject uiInventory;
    public GameObject uiShop;
    public TextMeshProUGUI moneyText;
    public int money;

    [Header("Tile functions")]
    [SerializeField]
    private Tilemap bottomLayer;
    [SerializeField]
    private Tilemap tilledLayer;
    [SerializeField]
    private Tilemap wateredLayer;
    [SerializeField]
    private Tilemap topLayer;
    [SerializeField]
    private TileBase tilledTile;
    [SerializeField]
    private TileBase wateredTile;

    [Header("Farming functions")]
    [SerializeField]
    private GameObject cropPrefab;

    [System.Serializable]
    public class InventoryItem
    {
        public Item item;
        public int quantity;

        public InventoryItem(Item itemReference, int amount, int location)
        {
            item = itemReference;
            quantity = amount;
        }
    }

    [Header("Inventory Debug")]
    public InventoryItem[] debugInventory;
    public Dictionary<int, Item> inventory = new Dictionary<int, Item>();
    public int inventorySize;

    private void Awake()
    {

        controls = new PlayerController();

        controls.Player.Move.performed += ctx => move = ctx.ReadValue<Vector2>();
        controls.Player.Move.performed += ctx => _look = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => move = Vector2.zero;

        controls.Player.Aim.performed += ctx => aim = ctx.ReadValue<Vector2>();
        controls.Player.Aim.canceled += ctx => aim = Vector2.zero;

        controls.Player.Menu.performed += ctx => paused = !paused;

        

        _tileSelector.transform.parent = null;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        for (int i = 0; i < debugInventory.Length; i++)
        {

            if (debugInventory[i].item != null)
            {
                if (!inventory.ContainsKey(i))
                {


                    Item item = Instantiate(debugInventory[i].item);
                    item.currentStack = debugInventory[i].quantity;

                    inventory.Add(i, item);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);

        if (inventory.ContainsKey(_itemSelector.selectedSlot))
        {
            _selectedItem = inventory[_itemSelector.selectedSlot];
        }
        else
        {
            _selectedItem = null;
        }

        if (paused)
        {

            Time.timeScale = 0;
            uiInventory.SetActive(true);
            moneyText.text = "$" + money.ToString();

        }
        else { 

            Time.timeScale = 1;
            uiInventory.SetActive(false);
        }

        if (!canMove) { return; }

        if (controls.Player.Interact.triggered)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Interact(true);
            }
            else
            {
                Interact(false);
            }
        }

        if (controls.Player.Alternate.triggered)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Back(true);
            }
            else
            {
                Back(false);
            }
        }
    }

    private void FixedUpdate()
    {
        float lookAngle = Mathf.Atan2(_look.y, _look.x);
        Vector2 pos = transform.position;

        _selector.transform.eulerAngles = new Vector3(0, 0, lookAngle * Mathf.Rad2Deg);
        _selector.transform.position = pos + new Vector2(Mathf.Cos(lookAngle), Mathf.Sin(lookAngle) - 0.5f) * 1.5f;

        _tileSelector.transform.position = new Vector2(Mathf.RoundToInt(_selector.transform.position.x), Mathf.RoundToInt(_selector.transform.position.y));

        int speedMod = 1;

        if (!canMove) speedMod = 0;

        float targetSpeedX = move.x * _speed * speedMod;

        float speedDifX = targetSpeedX - rb.velocity.x;

        float accelRateX = (Mathf.Abs(targetSpeedX) > 0.01f) ? _acceleration : _deceleration;

        float movementX = Mathf.Pow(Mathf.Abs(speedDifX) * accelRateX, _velPower) * Mathf.Sign(speedDifX);

        float targetSpeedY = move.y * _speed * speedMod;

        float speedDifY = targetSpeedY - rb.velocity.y;

        float accelRateY = (Mathf.Abs(targetSpeedY) > 0.01f) ? _acceleration : _deceleration;

        float movementY = Mathf.Pow(Mathf.Abs(speedDifY) * accelRateY, _velPower) * Mathf.Sign(speedDifY);

        rb.AddForce(movementX * Vector2.right);
        rb.AddForce(movementY * Vector2.up);
    }

    void Interact(bool mouseInput)
    {


        Collider2D[] colliders = Physics2D.OverlapAreaAll(_bound1.transform.position, _bound2.transform.position);

        foreach(Collider2D collider in colliders)
        {
            if(collider.TryGetComponent(out Interaction interaction))
            {
                if(interaction != null) {

                    Debug.Log("Interacting with something");
                    interaction.Interact(this);
                    return;

                }
            }
        }

        Vector3Int tileSelectorPosition = new Vector3Int(Mathf.RoundToInt(_tileSelector.transform.position.x), Mathf.RoundToInt(_tileSelector.transform.position.y));
        Vector2 adjustPlayerPos = new Vector2(transform.position.x, transform.position.y - 1);

        if (mouseInput)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Mathf.RoundToInt(Vector2.Distance(adjustPlayerPos, mousePosition)) <= 2) tileSelectorPosition = new Vector3Int(Mathf.FloorToInt(mousePosition.x), Mathf.FloorToInt(mousePosition.y)); else return;
        }

        if (!paused) InteractUnpaused(tileSelectorPosition);
        if (!paused) InteractTool(tileSelectorPosition);

    }

    void InteractTool(Vector3Int tileSelectorPosition)
    {
        if (_selectedItem == null) return;

        TileBase selectedTile = bottomLayer.GetTile(tileSelectorPosition);

        if(_selectedItem.type == Item.Type.TOOL)
        {
            if (_selectedItem.name.Contains("Axe"))
            {
                if (ChopTree(tileSelectorPosition)) return;
            }

            if (TileManager.instance.objectTiles.ContainsKey(tileSelectorPosition))
            {
                if(TileManager.instance.objectTiles[tileSelectorPosition].TryGetComponent(out BreakableObject breakableObject))
                {
                    breakableObject.Hit(_selectedItem);

                    return;
                }
            }
        }

        if (_selectedItem.name.Contains("Hoe") && _selectedItem.type == Item.Type.TOOL)
        {
            if (CheckBuildLayers(tileSelectorPosition, ignoreTilled: true, ignorePlanted: true)) return;

            if (selectedTile != null)
            {
                print("At position " + tileSelectorPosition + " there is a " + selectedTile);
                print(selectedTile + " tillable? " + TileManager.instance.dataFromTiles[selectedTile].tillable);

                if (topLayer.GetTile(tileSelectorPosition) == null)
                {
                    if (tilledLayer.GetTile(tileSelectorPosition) == null)
                    {
                        tilledLayer.SetTile(tileSelectorPosition, tilledTile);
                    }
                }
            }
            return;
        }

        if (_selectedItem.name.Contains("Watering Can") && _selectedItem.type == Item.Type.TOOL)
        {
            if (tilledLayer.GetTile(tileSelectorPosition) != null)
            {
                wateredLayer.SetTile(tileSelectorPosition, wateredTile);
                TileManager.instance.WaterTile(tileSelectorPosition);
            }
            return;
        }

        if (_selectedItem.name.Contains("Pickaxe") && _selectedItem.type == Item.Type.TOOL)
        {
            if (wateredLayer.GetTile(tileSelectorPosition) != null) wateredLayer.SetTile(tileSelectorPosition, null);
            tilledLayer.SetTile(tileSelectorPosition, null);

            TileManager.instance.RemoveWaterTile(tileSelectorPosition);
            TileManager.instance.RemoveCrop(tileSelectorPosition);

            return;
        }

        
    }

    bool ChopTree(Vector3Int tileSelectorPosition)
    {
        Vector2 checkPos = new Vector2(tileSelectorPosition.x + 0.5f, tileSelectorPosition.y + 0.5f);

        if (TileManager.instance.treeTiles.ContainsKey(tileSelectorPosition))
        {
            Debug.Log("Found a tree");

            Tree stc = TileManager.instance.treeTiles[tileSelectorPosition].GetComponent<Tree>();

            stc.Chop(this);

            return true;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPos, 1f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("FallenTree"))
            {
                collider.GetComponent<Tree>().Chop(this);
                return true;
            }
        }

        return false;
    }

    void InteractUnpaused(Vector3Int tileSelectorPosition)
    {
        if (_selectedItem != null)
        {
            if (_selectedItem.placeable == true)
            {
                if (CheckBuildLayers(tileSelectorPosition, ignoreTop: true)) return;

                _selectedItem.currentStack -= 1;

                var placedItem = new GameObject(_selectedItem.name);
                placedItem.transform.position = tileSelectorPosition;
                placedItem.transform.parent = TileManager.instance.hierarchyPlacedObjects.transform;
                placedItem.AddComponent<Item_Overworld>().item = Instantiate(_selectedItem);
                placedItem.GetComponent<Item_Overworld>().item.currentStack = 1;

                TileManager.instance.objectTiles.Add(tileSelectorPosition, placedItem);

                return;
            }

            if (_selectedItem.GetType() == typeof(Crop))
            {

                if (tilledLayer.GetTile(tileSelectorPosition) == null) return;
                if (TileManager.instance.plantedTiles.ContainsKey(tileSelectorPosition)) return;

                var placedCrop = Instantiate(cropPrefab, tileSelectorPosition, Quaternion.identity, TileManager.instance.hierarchyCrops.transform);
                placedCrop.GetComponent<CropTile>().crop = Instantiate((Crop)_selectedItem);

                _selectedItem.currentStack -= 1;
                TileManager.instance.plantedTiles.Add(tileSelectorPosition, placedCrop);

                return;
            }
        }
    }

    void InteractPaused(bool mouseInput)
    {
        if (!mouseInput)
        {
                _inventorySelector.ClickItem(_inventorySelector.selectedSlot);
        }
    }

    void Back(bool mouseInput)
    {
        Vector3Int tileSelectorPosition = new Vector3Int(Mathf.RoundToInt(_tileSelector.transform.position.x), Mathf.RoundToInt(_tileSelector.transform.position.y));
        Vector2 adjustPlayerPos = new Vector2(transform.position.x, transform.position.y - 1);


        if (mouseInput)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Mathf.RoundToInt(Vector2.Distance(adjustPlayerPos, mousePosition)) <= 2) tileSelectorPosition = new Vector3Int(Mathf.RoundToInt(mousePosition.x), Mathf.RoundToInt(mousePosition.y)); else return;
        }

        if (!paused) BackUnpaused(tileSelectorPosition);
    }

    void BackUnpaused(Vector3Int tileSelectorPosition)
    {
        if (TileManager.instance.plantedTiles.ContainsKey(tileSelectorPosition))
        {
            if (!TileManager.instance.plantedTiles[tileSelectorPosition].GetComponent<CropTile>().readyToHarvest) return;

            TileManager.instance.plantedTiles[tileSelectorPosition].GetComponent<CropTile>().Harvest(this);
        }

        if (_selectedItem == null || 
            (
            TileManager.instance.objectTiles.ContainsKey(tileSelectorPosition) && 
            TileManager.instance.objectTiles[tileSelectorPosition].GetComponent<Item_Overworld>().item.name == _selectedItem.name
            ))
        {
            if (!TileManager.instance.objectTiles.ContainsKey(tileSelectorPosition)) return;

            if(AddItemToInventory(TileManager.instance.objectTiles[tileSelectorPosition].GetComponent<Item_Overworld>().item)) 
                TileManager.instance.RemoveObject(tileSelectorPosition);
        }
    }

    public bool AddItemToInventory(Item item, int quantity = 1)
    {

        Item itemInstance = Instantiate(item);
        itemInstance.currentStack = quantity;

        foreach(KeyValuePair<int, Item> entry in inventory)
        {
            if(entry.Value.name == item.name)
            {
                if(entry.Value.currentStack < entry.Value.maxStack)
                {

                    int spaceLeft = entry.Value.maxStack - entry.Value.currentStack;

                    if (spaceLeft - item.currentStack < 0)
                    {
                        entry.Value.currentStack += spaceLeft;
                        itemInstance.currentStack -= spaceLeft;

                    }
                    else
                    {
                        entry.Value.currentStack += itemInstance.currentStack;
                        return true;
                    }
                }
            }
        }

        bool added = false;

        for (int i = 0; i < inventorySize; i++)
        {
            if (!inventory.ContainsKey(i))
            {
                inventory.Add(i, itemInstance); 
                added = true;
            }

            if (added) return true;
        }

        //Unable to add item to inventory

        ItemManager.instance.CreateDroppedItem(itemInstance, transform.position);

        return false;
    }

    public bool RemoveItemFromInventory(Item item)
    {
        foreach (KeyValuePair<int, Item> entry in inventory)
        {
            if (entry.Value.name == item.name)
            {
                entry.Value.currentStack -= 1;
                return true;
            }
        }

        return false;
    }

    bool CheckBuildLayers(Vector3Int tileSelectorPosition, bool ignoreTilled = false, bool ignoreTop = false, bool ignorePlanted = false, bool ignoreObject = false) {

        if (!ignoreTilled && tilledLayer.GetTile(tileSelectorPosition) != null) return true;
        if (!ignoreTop && topLayer.GetTile(tileSelectorPosition) != null) return true;
        if (!ignorePlanted && TileManager.instance.plantedTiles.ContainsKey(tileSelectorPosition)) return true;
        if (!ignoreObject && TileManager.instance.objectTiles.ContainsKey(tileSelectorPosition) && TileManager.instance.treeTiles.ContainsKey(tileSelectorPosition)) return true;

        return false;
    }
}
