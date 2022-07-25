using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Item_Overworld : MonoBehaviour
{
    public Item item;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] GameObject shadowRenderer;

    void Start()
    {
        if (!TryGetComponent(out SpriteRenderer sr))
        {
            gameObject.AddComponent<SpriteRenderer>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (item == null)
        {
            item = ItemManager.instance.items.ToList().Find(i => i.name == "Suspicious Seed");

            spriteRenderer.sprite = item.icon;
        }
        else
        {
            if (item.overworldSprite != null) spriteRenderer.sprite = item.overworldSprite;
            else spriteRenderer.sprite = item.icon;
        }

        if(shadowRenderer == null)
        {
            var shadow = new GameObject(item.name + " shadow");
            shadow.transform.parent = transform;
            shadow.transform.position = transform.position;
            shadow.AddComponent<SpriteRenderer>().sprite = ItemManager.instance.itemShadow;
            shadow.GetComponent<SpriteRenderer>().sortingLayerName = "Layer2";
            shadow.GetComponent<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt(transform.position.y * 10) - 1;

        }
        spriteRenderer.sortingLayerName = "Layer2";
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Player player))
        {
            player.AddItemToInventory(item);
        }
    }

    public void Remove()
    {
        Debug.Log("Removing self");
        Destroy(gameObject);
    }
}
