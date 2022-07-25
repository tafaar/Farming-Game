using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DebugGiver : MonoBehaviour, IPointerClickHandler
{
    public Player player;
    public Item item;

    private void Start()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = item.icon;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        player.AddItemToInventory(Instantiate(item));
    }
}
