using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TreeFall : Tree
{
    public GameObject stumpRef;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] BoxCollider2D collider;
    [SerializeField] ParticleSystem fallPs;
    [SerializeField] ParticleSystemRenderer fallPsR;
    [SerializeField] GameObject leafCenter;

    public Tree treeRef;

    public bool atTargetY;
    public bool atTargetA;
    bool atTarget;

    [SerializeField] float targetDelay;
    [SerializeField] float rotationTorque;
    float delayTime;
    float fallTime;

    float startTime;
    bool started;

    public int fallDir = -1;

    Vector2 fallLocation;

    private void Awake()
    {
        print("falling");
    }

    private void Start()
    {

        if (treeRef != null)
        {
            string treeTopName = treeRef.GetComponent<SpriteRenderer>().sprite.name + "_Top";

            Debug.Log(treeTopName);

            spriteRenderer.sprite = TileManager.instance.treeSprites.ToList().Find(sprite => sprite.name.Equals(treeTopName));

            health = 20f;

            fallLocation = stumpRef.transform.position;
        }

        if(leaves != null)
        {
            fallPsR.material = leaves.GetComponent<SpriteRenderer>().material;
            leaves.GetComponent<SpriteRenderer>().color = Color.white;
            leaves.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderer.sortingOrder + 20;
        }

        fallPs.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        startTime += Time.deltaTime;

        if (startTime < 2f) return;

        if (!started)
        {
            spriteRenderer.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10) + 1;

            collider.enabled = true;

            Vector2 pos = transform.position;
            started = true;

        }

        if(delayTime <= targetDelay) delayTime += Time.deltaTime;
        if (delayTime >= targetDelay) fallTime += Time.deltaTime;

        if(transform.position.y <= fallLocation.y)
        {
            atTargetY = true;
        }

        if(atTargetY)
        {

            if(fallDir == -1 && transform.rotation.eulerAngles.z <= 270f) atTargetA = true;
            if (fallDir == 1 && transform.rotation.eulerAngles.z >= 90f) atTargetA = true;

        }

        float angle = (360f - transform.rotation.eulerAngles.z) * Mathf.Deg2Rad;

        collider.size = new Vector2(collider.size.x, 4.5f * ((fallDir == -1) ? Mathf.Sin(angle) : Mathf.Sin(angle + Mathf.PI)));

        collider.offset = new Vector2(collider.offset.x, (collider.size.y / 2f) + 0.75f);

        if (atTargetY && atTargetA) atTarget = true;

        if (!atTarget)
        {
            if (atTargetY) rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            if (atTargetA) rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (atTarget) rb.constraints = RigidbodyConstraints2D.FreezeAll;

    }

    private void FixedUpdate()
    {
        if (!atTargetA) rb.angularVelocity += rotationTorque * Mathf.Pow(startTime, 2f) * fallDir;

        if (!atTargetY) rb.velocity = new Vector2(rb.velocity.x, -2f * Mathf.Pow(fallTime, 2f));
    }

    public override void Chop(Player player)
    {
        if (!atTarget) return;

        health -= 10f;

        if (health <= 0) Fall(player);
    }

    public override void Fall(Player player)
    {
        fallPsR.transform.position = leafCenter.transform.position;
        fallPsR.sortingOrder = spriteRenderer.sortingOrder + 20;
        fallPs.Play();
        fallPs.GetComponent<DelayedDestroy>().Remove(6f);

        for (int i = 0; i < Random.Range(12, 17); i++) {
            ItemManager.instance.CreateDroppedItem(ItemManager.instance.ItemByName("Wood"), leafCenter.transform.position);
        }
        Destroy(gameObject);
    }
}
