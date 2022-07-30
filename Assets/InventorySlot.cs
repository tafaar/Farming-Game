using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Player player;
    [SerializeField] private Image itemIcon;
    [SerializeField] private ItemBarSelector selector;
    [SerializeField] private GameObject textParent;
    [SerializeField] private Color selectedColor;
    public int slotID;

    private Item item;
    int stackSize;
    TextMeshProUGUI stackText;
    Image image;
    float _deselectTime;

    private void Awake()
    {
        _deselectTime = 5f;
        image = GetComponent<Image>();
        stackText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        DisplayItem();
    }

    private void OnEnable()
    {
        DisplayItem();
    }

    private void Update()
    {
        DisplayItem();

        if (item != null && item.currentStack < 1)
        {
            Destroy(item);
            player.inventory.Remove(slotID);
        }

        Vector2 halfScale = new Vector2(0.5f, 0.5f);
        Vector2 fullScale = new Vector2(0.75f, 0.75f);

        if(selector.selectedSlot == slotID)
        {
            if(selector.mousingOver == false) selector.highlightedItem = item;

            itemIcon.transform.localScale = Vector2.Lerp(itemIcon.transform.localScale, fullScale, 0.5f);
            image.color = selectedColor;

            if(_deselectTime != 0) _deselectTime = 0;
        }
        else
        {
            _deselectTime += Time.unscaledDeltaTime;

            itemIcon.transform.localScale = Vector2.Lerp(itemIcon.transform.localScale, halfScale, 0.5f * _deselectTime);
            image.color = Color.Lerp(selectedColor, Color.white, 3 * _deselectTime);
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(selector.type == ItemBarSelector.Type.INVENTORY)
        {
            if (item != null) selector.highlightedItem = item;
            selector.selectedSlot = slotID;

            selector.mousingOver = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selector.type == ItemBarSelector.Type.INVENTORY)
        {
            selector.mousingOver = false;

            if (selector.selectedSlot == slotID) selector.highlightedItem = item;
        }
    }

    void DisplayItem()
    {
        if (player.inventory.ContainsKey(slotID))
        {
            var refItem = player.inventory[slotID];

            item = refItem;
            stackSize = refItem.currentStack;
            itemIcon.sprite = refItem.icon;
            itemIcon.enabled = true;
        }
        else
        {
            item = null;
            stackSize = -1;
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        stackText.text = stackSize.ToString();
        if (stackSize <= 1) stackText.text = "";
    }
}
