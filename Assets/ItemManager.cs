using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteAlways]
public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    public Item[] items;
    public Sprite itemShadow;
    public GameObject itemDropPrefab;
    [SerializeField] GameObject droppedItems;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        items = items.OrderBy(item => item.name).ToArray();
    }

    private void Update()
    {
        if (Application.IsPlaying(this)) return;
        items = items.OrderBy(item => item.name).ToArray();

    }

    public Item ItemByName(string name)
    {
        Debug.Log("Searching for an item by name of " + name);

        Item returnItem = Instantiate(items.ToList().Find(item => item.name.Equals(name)));

        return returnItem;
    }

    public GameObject CreateDroppedItem(Item item, Vector2 position, int quantity = 1)
    {
        Item chosenItem = Instantiate(item);

        GameObject newDrop = Instantiate(itemDropPrefab, position, Quaternion.identity);

        chosenItem.currentStack = quantity;

        newDrop.GetComponent<DroppedItem>().item = chosenItem;
        newDrop.GetComponent<DroppedItem>().Pop();

        return newDrop;
    }
}
