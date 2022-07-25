using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemBarSelector : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject[] itemSlots;
    [SerializeField] ToolTipUI toolTip;
    public enum Type { INVENTORY, HOTBAR }
    public Type type;
    public int selectedSlot = 0;
    public Item storedItem;
    public Item highlightedItem;
    bool holdingItem;
    bool mouseHolding;
    int oldSlot;

    [Header("Inventory Only")]
    public Image heldItem;
    public TextMeshProUGUI heldText;

    bool runningCr;
    bool initialInput;

    public bool mousingOver;

    float _mouseScroll;

    [Header("Shop Stuff")]
    public bool selling = false;

    // Start is called before the first frame update
    void Start()
    {
        player.controls.Player.Aim.canceled += ctx => initialInput = true;


        transform.position = itemSlots[0].transform.position;

    }

    // Update is called once per frame
    void Update()
    {

        _mouseScroll = Input.mouseScrollDelta.y;

        if (storedItem != null && storedItem.currentStack < 1)
        {
            ClearHeldItem();
        }

        if ((player.paused && type == Type.INVENTORY) || (!player.paused && type == Type.HOTBAR))
        {
            if (player.controls.Player.Aim.triggered) { runningCr = false; StopAllCoroutines(); initialInput = true; }
            if (!runningCr) StartCoroutine(MoveMenu());
        }

        transform.position = itemSlots[selectedSlot].transform.position;

  
        if(type == Type.HOTBAR)
        {
            for (int i = 0; i < 10; i++)
            {
                KeyCode numDown = (KeyCode)(48 + i);

                if (Input.GetKeyDown(numDown))
                {
                    selectedSlot = i - 1;

                    if (i == 0) selectedSlot = itemSlots.Length - 1;
                }
            }
        }

        if (type == Type.INVENTORY)
        {
            if (player.controls.Player.Interact.triggered)
            {
                mouseHolding = false;
                ClickItem(selectedSlot, 0);
            }

            if (player.controls.Player.Alternate.triggered)
            {
                mouseHolding = false;
                ClickItem(selectedSlot, 1);
            }

            if (highlightedItem != null)
            {
                UpdateToolTip(highlightedItem);
            }
            else
            {
                toolTip.GetComponent<CanvasGroup>().alpha = 0;
            }

            if (holdingItem)
            {
                heldItem.gameObject.SetActive(true);

                if (heldItem != null)
                {
                    heldItem.sprite = storedItem.icon;

                    if (storedItem.currentStack > 1)
                    {
                        heldText.text = storedItem.currentStack.ToString();
                    }
                    else heldText.text = "";
                }
                else heldItem.sprite = null;

                if(mouseHolding) heldItem.transform.position = Input.mousePosition;
                if (!mouseHolding) heldItem.transform.position = transform.position;
            }
            else heldItem.gameObject.SetActive(false);
        }
    }

    //For mouse interaction
    public void SelectThisSlot(int itemSlot, int clickType)
    {
        selectedSlot = itemSlot;

        mouseHolding = true;

        Debug.Log("Selecting slot " + itemSlot);

        if (type == Type.INVENTORY) {

            if (!selling) { ClickItem(itemSlot, clickType); return; }
            if(selling) { Sell(itemSlot, clickType); return; }

        }
    }

    public void UpdateToolTip(Item item)
    {
        toolTip.itemSprite.sprite = item.icon;
        toolTip.itemName.text = item.name;
        toolTip.itemDescription.text = item.description;

        string typeString = item.type.ToString().ToLower();
        typeString = char.ToUpper(typeString[0]) + typeString[1..];

        toolTip.itemType.text = typeString;

        if (item.name.Contains("Seed")) toolTip.itemType.text = "Seed";

        toolTip.GetComponent<CanvasGroup>().alpha = 1;
    }

    IEnumerator MoveMenu()
    {
        runningCr = true;

        Vector2 scroll = player.aim;

        scroll.x += _mouseScroll;

        if (scroll.x < 0)
        {

            selectedSlot -= 1;

            if (type == Type.HOTBAR)
            {
                if (selectedSlot < 0)
                {
                    selectedSlot = itemSlots.Length - 1;
                }
            }
            else
            {
                if (selectedSlot % 10 == 9 || selectedSlot == -1)
                {
                    selectedSlot += 10;
                }
            }

        };
        if (scroll.x > 0)
        {

            selectedSlot += 1;

            if (type == Type.HOTBAR)
            {
                if (selectedSlot >= itemSlots.Length)
                {
                    selectedSlot = 0;
                }
            }
            else
            {
                if (selectedSlot % 10 == 0)
                {
                    selectedSlot -= 10;
                }
            }

        };
        if (scroll.y > 0)
        {

            selectedSlot -= 10;

            if (selectedSlot < 0)
            {
                selectedSlot = itemSlots.Length + selectedSlot;
            }

        };

        if (scroll.y < 0)
        {

            selectedSlot += 10;

            if (selectedSlot >= itemSlots.Length)
            {
                selectedSlot -= itemSlots.Length;
            }

        };

        if (initialInput) { initialInput = false; yield return new WaitForSecondsRealtime(0.14f); }
        yield return new WaitForSecondsRealtime(0.8f);

        runningCr = false;

    }

    public void Sell(int itemSlot, int clickType = 0)
    {

        // Sell whole stack
        if (clickType == 0)
        {
            if (player.inventory.ContainsKey(itemSlot))
            {
                Item sellItem = player.inventory[itemSlot];

                if (sellItem == null) return;

                player.money += sellItem.basePrice * sellItem.currentStack;

                player.inventory.Remove(itemSlot);

            }
            else
            {
                return;
            }
        }

        // Sell 1 in the stack
        if (clickType == 1)
        {
            if (player.inventory.ContainsKey(itemSlot))
            {
                Item sellItem = player.inventory[itemSlot];

                if (sellItem == null) return;

                player.money += sellItem.basePrice;

                player.inventory[itemSlot].currentStack -= 1;

                if (player.inventory[itemSlot].currentStack < 1) player.inventory.Remove(itemSlot);
            }
            else
            {
                return;
            }
        }
    }

    public void ClickItem(int itemSlot, int clickType = 0)
    {
        if (type == Type.INVENTORY)
        {
            if (selling) { Sell(itemSlot, clickType); return; }
        }

            // Click type 0 is left, 1 is right, 2 is middle
            if (clickType == 0)
        {
            if (!holdingItem)
            {
                if (player.inventory.ContainsKey(itemSlot))
                {
                    oldSlot = itemSlot;
                    storedItem = player.inventory[itemSlot];

                    Debug.Log("Storing " + storedItem.name);

                    player.inventory.Remove(itemSlot);

                    holdingItem = true;
                }
                return;
            }

            if (holdingItem)
            {
                if (!player.inventory.ContainsKey(itemSlot))
                {
                    // For when the player has an empty slot at itemSlot

                    player.inventory.Add(itemSlot, storedItem);
                    ClearHeldItem();
                }
                else
                {

                    // Behavior if the slot contains an item

                    if (player.inventory[itemSlot].name == storedItem.name)
                    {

                        // If the item matches the held item

                        var itemMatch = player.inventory[itemSlot];

                        if (itemMatch.currentStack < itemMatch.maxStack)
                        {

                            // If the item in the inventory is not at max stack

                            int stackDiff = itemMatch.maxStack - itemMatch.currentStack;

                            if (storedItem.currentStack <= stackDiff)
                            {

                                //If the held item quantity is less than the stack difference

                                itemMatch.currentStack += storedItem.currentStack;
                                ClearHeldItem();
                                return;
                            }
                            else
                            {

                                // If the held item quantity is greater than the stack difference
                                // Add the difference to the inventory item, subtract it from the held item

                                itemMatch.currentStack += stackDiff;
                                storedItem.currentStack -= stackDiff;
                                return;
                            }
                        }

                        // If the item is at max stack, continue behavior

                    }

                    // Swap the held item and the inventory item

                    Item temp = storedItem;

                    storedItem = player.inventory[itemSlot];

                    player.inventory[itemSlot] = temp;
                }
                return;
            }
        }

        if (clickType == 1)
        {
            if(!holdingItem)
            {
                if (!player.inventory.ContainsKey(itemSlot)) return;

                Item selectedItem = Instantiate(player.inventory[itemSlot]);

                int quantity = selectedItem.currentStack;

                selectedItem.currentStack = Mathf.CeilToInt(quantity / 2f);

                player.inventory[itemSlot].currentStack = Mathf.FloorToInt(quantity / 2f);

                storedItem = selectedItem;
                holdingItem = true;

                return;
            }

            if (holdingItem)
            {
                if (!player.inventory.ContainsKey(itemSlot))
                {
                    // If the right-clicked slot is empty, add one to the slot and remove from the held item

                    Item itemInstance = Instantiate(storedItem);

                    itemInstance.currentStack = 1;
                    storedItem.currentStack -= 1;

                    player.inventory.Add(itemSlot, itemInstance);

                    return;
                }
                else
                {

                    // If it's not empty, check if it's a matching item with available space, otherwise return

                    if (player.inventory[itemSlot].name == storedItem.name)
                    {

                        Item itemMatch = player.inventory[itemSlot];

                        if(itemMatch.currentStack < itemMatch.maxStack)
                        {

                            itemMatch.currentStack += 1;
                            storedItem.currentStack -= 1;

                        }
                    }

                    return;
                }
            }
            
        }
    }
    void ClearHeldItem()
    {
        storedItem = null;
        holdingItem = false;
    }

    public void DropHeldItem()
    {
        if (storedItem == null) return;
        if (!storedItem.canBeDropped) return;

        var droppedItem = ItemManager.instance.CreateDroppedItem(storedItem, player.transform.position, storedItem.currentStack);
        droppedItem.GetComponent<DroppedItem>().targetTime = 3f;
        ClearHeldItem();
    }

    public void ToggleSellMode() {

        selling = !selling;

        print("Selling: " + selling);
    }
}
