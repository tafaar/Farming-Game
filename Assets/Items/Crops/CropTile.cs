using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropTile : MonoBehaviour
{
    public Crop crop;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    public bool readyToHarvest;

    private Dictionary<int, Sprite> growthChange = new Dictionary<int, Sprite>();

    bool seedOffset = false;

    void Start()
    {
        spriteRenderer.sprite = crop.overworldSprite;

        Debug.Log("There are " + crop.growthDays.Length + " growth days and " + crop.growthSprites.Length + " growth sprites.");

        for (int i = 0; i < crop.growthSprites.Length; i++)
        {
            growthChange.Add(crop.growthDays[i], crop.growthSprites[i]);
        }

        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10) - Mathf.RoundToInt(crop.sortOffset * 10) - 10;
        
    }

    public void Grow()
    {
        crop.currentDays += 1;

        if (growthChange.ContainsKey(crop.currentDays))
        {
            if (seedOffset == false)
            {
                spriteRenderer.sortingOrder = -Mathf.RoundToInt(spriteRenderer.transform.position.y * 10);
                seedOffset = true;
            } 
            spriteRenderer.sprite = growthChange[crop.currentDays];
        }
        if (crop.currentDays >= crop.daysToGrow) readyToHarvest = true;
    }

    public void Harvest(Player player)
    {
        Vector3Int pos = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);

        if (player.AddItemToInventory(crop.harvestedCrop)) {

            if (crop.regrows == false)
            {
                TileManager.instance.RemoveCrop(pos);
            }

            if(crop.regrows == true)
            {
                crop.currentDays = crop.regrowDay;
                spriteRenderer.sprite = crop.regrowSprite;
                readyToHarvest = false;
            }
        }
    }
}
