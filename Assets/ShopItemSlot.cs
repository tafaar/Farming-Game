using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[ExecuteAlways]
public class ShopItemSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Player player;
    public Item item;
    public Image slotImage;
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemPrice;
    public TextMeshProUGUI itemQuantity;
    public Color selectColor;
    public Color unaffordableColor;
    public Color normalColor;
    public int stock = 1;

    public bool buying;
    public bool clicked;
    public float holdTimer;

    private void Start()
    {
        itemImage.sprite = item.icon;
        itemName.text = item.name;
        if (buying) itemPrice.text = "$" + item.basePrice.ToString();
        if (!buying) itemPrice.text = "$" + Mathf.FloorToInt(item.basePrice * 0.8f).ToString();
        itemQuantity.text = "x" + stock.ToString();
    }

    private void Update()
    {
        itemQuantity.text = "x" + stock.ToString();

        if (buying)
        {
            if (player.money >= item.basePrice)
            {
                slotImage.color = normalColor;

                if (clicked) { holdTimer += Time.unscaledDeltaTime; } else holdTimer = 0;

                if (holdTimer >= 1.05f)
                {
                    Buy();
                    holdTimer = 1f;
                }
            }
            else
            {
                holdTimer = 0;
                slotImage.color = unaffordableColor;
            }
        }

        if (!buying)
        {
            if (clicked) { holdTimer += Time.unscaledDeltaTime; } else holdTimer = 0;

            if (holdTimer >= 1.05f)
            {
                Sell();
                holdTimer = 1f;
            }

            if(item.canBeSold == false)
            {
                slotImage.color = unaffordableColor;
            }
        }

        if(stock < 1)
        {
            Destroy(gameObject);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        clicked = true;
        if(buying) Buy();
        if (!buying) Sell();
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        clicked = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        slotImage.color = selectColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        slotImage.color = normalColor;
    }

    public void Buy()
    {
        if(player.money >= item.basePrice)
        {
            player.money -= item.basePrice;
            player.AddItemToInventory(item, 1);
            print(player.name + "bought " + item.name);
        }
        else
        {
            print("Can't afford " + item.name);
        }
    }

    public void Sell()
    {
        if (!item.canBeSold) return;

        player.money += Mathf.FloorToInt(item.basePrice * 0.8f);

        foreach(KeyValuePair<int, Item> invSlot in player.inventory)
        {
            if(invSlot.Value.name == item.name)
            {
                player.RemoveItemFromInventory(item);
                stock -= 1;
            }
        }
    }

}
