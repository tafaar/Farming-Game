using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public List<Item> shopItems;
    public virtual bool Interact(Player player)
    {
        print("Interacting");

        player.menuState = Player.MenuState.SHOP;
        player.uiShop.GetComponent<ShopMenu>().items = shopItems;
        player.uiShop.SetActive(true);

        return true;
    }
}
