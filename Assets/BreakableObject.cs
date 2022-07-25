using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BreakableObject : MonoBehaviour
{
    public enum ObjectType { ROCK, ROCK_BIG, WOOD, WOOD_BIG}
    public ObjectType type;

    public float health;
    public SpriteRenderer spriteRenderer;

    public string[] acceptedTools;

    public Item[] drops;
    public int[] quantities;
    public float[] probabilities;

    public bool randomSprite = true;

    List<Vector3Int> occupiedTiles = new List<Vector3Int>();

    // Update is called once per frame

    public void Awake()
    {
        Vector3Int pos = Vector3Int.RoundToInt(transform.position);

        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);

        if (type == ObjectType.ROCK_BIG)
        {
            Vector3Int[] otherTiles = { new Vector3Int(1, 0), new Vector3Int(0, 1), new Vector3Int(1, 1) };

            for (int i = 0; i < otherTiles.Length; i++)
            {
                occupiedTiles.Add(pos + otherTiles[i]);
            }
        }

        if (type == ObjectType.WOOD_BIG)
        {
            occupiedTiles.Add(pos + new Vector3Int(1, 0));
        }

        if (randomSprite)
        {

            List<Sprite> possibleSprites = new();

            if (type == ObjectType.ROCK)
            {
                foreach (Sprite sprite in TileManager.instance.rockSprites)
                {
                    if (sprite.name.Contains("Small")) possibleSprites.Add(sprite);
                }
            }

            if (type == ObjectType.ROCK_BIG)
            {
                foreach (Sprite sprite in TileManager.instance.rockSprites)
                {
                    if (sprite.name.Contains("Big")) possibleSprites.Add(sprite);
                }
            }

            if (type == ObjectType.WOOD)
            {
                foreach (Sprite sprite in TileManager.instance.woodSprites)
                {
                    if (sprite.name.Contains("Small")) possibleSprites.Add(sprite);
                }
            }

            if (type == ObjectType.WOOD_BIG)
            {
                foreach (Sprite sprite in TileManager.instance.woodSprites)
                {
                    if (sprite.name.Contains("Big")) possibleSprites.Add(sprite);
                }
            }

            int rand = Random.Range(0, possibleSprites.Count);

            spriteRenderer.sprite = possibleSprites[rand];

        }

        occupiedTiles.Add(pos);

        foreach (Vector3Int tile in occupiedTiles) TileManager.instance.objectTiles.Add(tile, gameObject);

    }

    public virtual bool Hit(Item selectedItem)
    {
        bool hit = false;

        foreach(string tool in acceptedTools)
        {
            if (selectedItem.name.Contains(tool) && selectedItem.type == Item.Type.TOOL) hit = true;
        }

        if (hit == false) { print("Wrong tool"); return false; }

        health -= 10f;

        if (health <= 0) Break();

        return true;

    }

    public virtual void Break()
    {
        for(int i = 0; i < drops.Length; i++)
        {
            float roll = Random.Range(0, 100f) / 100f;

            if (probabilities[i] >= roll)
            {
                for (int q = 0; q < quantities[i] + 1; q ++) {
                    var itemDrop = ItemManager.instance.CreateDroppedItem(drops[i], transform.position, 1);
                    itemDrop.transform.position += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
                }
            }
        }

        foreach (Vector3Int tile in occupiedTiles) TileManager.instance.objectTiles.Remove(tile);
        Destroy(gameObject);
    }
}
