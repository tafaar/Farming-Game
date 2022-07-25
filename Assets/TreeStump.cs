using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TreeStump : Tree
{
    public Tree treeRef;

    Vector3Int _pos;

    ItemManager itemManager;

    private void Awake()
    {
        print("adding stump");
    }

    private void Start()
    {
        _pos = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);

        if (treeRef != null)
        {
            string treeStumpName = treeRef.GetComponent<SpriteRenderer>().sprite.name + "_Stump";

            GetComponent<SpriteRenderer>().sprite = TileManager.instance.treeSprites.ToList().Find(sprite => sprite.name.Equals(treeStumpName));
        }
    }

    public override void Fall(Player player)
    {
        for (int i = 0; i < Random.Range(5, 8); i++)
        {
            ItemManager.instance.CreateDroppedItem(ItemManager.instance.ItemByName("Wood"), transform.position);
        }

        TileManager.instance.treeTiles.Remove(_pos);
        Destroy(gameObject);
    }
}
