using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DroppedItem : MonoBehaviour
{
    public Item item;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public Animator anim;
    public CircleCollider2D pickupTrigger;

    public float targetTime = 1f;
    float dropTime;
    bool canBePickedUp;

    void Start()
    {
        pickupTrigger.radius = 2f;

        if (item == null)
        {
            item = ItemManager.instance.items.ToList().Find(i => i.name == "Suspicious Seed");

            spriteRenderer.sprite = item.icon;
        }
        else
        {
            spriteRenderer.sprite = item.icon;
        }

        gameObject.name = "groundItem_" + item.name;
    }

    private void Update()
    {
        dropTime += Time.deltaTime;

        if (dropTime >= targetTime) canBePickedUp = true;

        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (canBePickedUp)
        {
            if (collider.gameObject.CompareTag("Player"))
            {
                Player player = collider.gameObject.GetComponent<Player>();

                //Search for an empty slot

                bool fullInventory = true;

                for(int i = 0; i < player.inventorySize; i++)
                {
                    
                    if (!player.inventory.ContainsKey(i))
                    {
                        fullInventory = false;
                        break;
                    }
                }

                if (fullInventory == true) return;

                //If there is an empty slot, continue

                player.AddItemToInventory(item, item.currentStack);

                Destroy(gameObject);
            }
        }
    }

    public void Pop(float flyForce = 1f)
    {
        anim.Play("item_Pop");

        rb.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f) * flyForce), ForceMode2D.Impulse);
    }
}
