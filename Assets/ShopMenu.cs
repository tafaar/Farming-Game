using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[ExecuteAlways]

public class ShopMenu : MonoBehaviour
{
    public Player player;
    public List<Item> items;
    public List<GameObject> displayedItems;
    public GameObject shopSection;
    [SerializeField] GameObject shopItemPrefab;
    [SerializeField] ScrollRect scroll;

    private void Update()
    {
        scroll.velocity = player.move * 200f;
    }

    void OnEnable()
    {
        player.canMove = false;

        foreach (GameObject displayedItem in displayedItems)
        {
            if (displayedItem != null) DestroyImmediate(displayedItem);
        }

        displayedItems = new();

        items = items.OrderBy(item => item.name).ToList();

        foreach(Item item in items)
        {
            Debug.Log(item.name);

            var itemPrefab = Instantiate(shopItemPrefab, shopSection.transform);

            itemPrefab.GetComponent<ShopItemSlot>().item = Instantiate(item); ;
            itemPrefab.GetComponent<ShopItemSlot>().player = player;

            displayedItems.Add(itemPrefab);
        }
    }

    private void OnDisable()
    {
        player.canMove = true;

        foreach(GameObject displayedItem in displayedItems)
        {
            DestroyImmediate(displayedItem);
        }

        displayedItems.Clear();

    }

    public void EnterBuyMode()
    {
        foreach (GameObject displayedItem in displayedItems)
        {
            if (displayedItem != null) DestroyImmediate(displayedItem);
        }

        displayedItems.Clear();

        displayedItems = new();

        items = items.OrderBy(item => item.name).ToList();

        foreach (Item item in items)
        {
            Debug.Log(item.name);

            var itemPrefab = Instantiate(shopItemPrefab, shopSection.transform);

            itemPrefab.GetComponent<ShopItemSlot>().buying = true;
            itemPrefab.GetComponent<ShopItemSlot>().item = Instantiate(item);
            itemPrefab.GetComponent<ShopItemSlot>().player = player;

            displayedItems.Add(itemPrefab);
        }
    }

    public void EnterSellMode()
    {
        foreach (GameObject displayedItem in displayedItems)
        {
            if(displayedItem != null) DestroyImmediate(displayedItem);
        }

        displayedItems.Clear();

        displayedItems = new();

        foreach(KeyValuePair<int, Item> inventorySlot in player.inventory)
        {
            Debug.Log(inventorySlot.Value.name);

            var selectedItem = inventorySlot.Value;

            var itemPrefab = Instantiate(shopItemPrefab, shopSection.transform);

            itemPrefab.GetComponent<ShopItemSlot>().buying = false;
            itemPrefab.GetComponent<ShopItemSlot>().item = Instantiate(selectedItem);
            itemPrefab.GetComponent<ShopItemSlot>().player = player;
            itemPrefab.GetComponent<ShopItemSlot>().stock = selectedItem.currentStack;

            displayedItems.Add(itemPrefab);
        }
    }

}
