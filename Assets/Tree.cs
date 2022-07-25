using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Tree : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float health;
    [SerializeField] GameObject treeTopPrefab;
    [SerializeField] GameObject treeStumpPrefab;
    public bool canBeChopped = true;
    public GameObject leaves;
    public ParticleSystem ps;
    public ParticleSystemRenderer psr;

    Vector3Int pos;

    // Start is called before the first frame update
    void Awake()
    {
        pos = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);

        transform.position = pos;

        TileManager.instance.treeTiles.Add(pos, gameObject);

        Debug.Log("Added a tree at " + pos);

        spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);

        if (leaves != null)
        {
            var leavesSr = leaves.GetComponent<SpriteRenderer>();

            leavesSr.sortingOrder = spriteRenderer.sortingOrder + 10;
            leavesSr.sprite = TileManager.instance.treeSprites.ToList().Find(sprite => sprite.name.Equals(spriteRenderer.sprite.name + "_Leaves"));

            int truncIndex = spriteRenderer.sprite.name.IndexOf("_", 5);

            string matName = spriteRenderer.sprite.name[..truncIndex] + "_LeafMat";

            leavesSr.material = TimeManager.instance.leafMaterials.ToList().Find(mat => mat.name.Equals(matName));


        }
    }

    public virtual void Chop(Player player)
    {
        if (!canBeChopped) return;

        health -= 10f;

        if (health <= 0) Fall(player);
    }

    public virtual void Fall(Player player)
    {
        ps.Play();
        psr.sortingOrder = spriteRenderer.sortingOrder;

        int fallDir = 1;

        if (transform.position.x <= player.transform.position.x) fallDir = 1;
        if (transform.position.x > player.transform.position.x) fallDir = -1;

        var newStump = Instantiate(treeStumpPrefab, transform.position, Quaternion.identity);
        newStump.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder - 1;
        newStump.GetComponent<TreeStump>().treeRef = this;
        TileManager.instance.treeTiles[pos] = newStump;

        var newTop = Instantiate(treeTopPrefab, transform.position + new Vector3(0.5f, 0, 0), Quaternion.identity);
        newTop.GetComponent<TreeFall>().stumpRef = newStump;
        newTop.GetComponent<TreeFall>().treeRef = this;
        newTop.GetComponent<TreeFall>().fallDir = fallDir;
        newTop.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder;
        leaves.transform.parent = newTop.transform;
        newTop.GetComponent<TreeFall>().leaves = leaves;

        for (int i = 0; i < Random.Range(1, 3); i++)
        {
            ItemManager.instance.CreateDroppedItem(ItemManager.instance.ItemByName("Wood"), transform.position);
        }

        Destroy(gameObject);
    }
}
