using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;

    [SerializeField]
    public List<TileData> tileDatas;
    [SerializeField]
    Tilemap wateredLayer;

    public Sprite[] treeSprites;
    public Sprite[] rockSprites;
    public Sprite[] woodSprites;

    public GameObject hierarchyCrops;
    public GameObject hierarchyPlacedObjects;

    public Dictionary<TileBase, TileData> dataFromTiles;
    public Dictionary<Vector3Int, bool> wateredTiles = new Dictionary<Vector3Int, bool>();
    public Dictionary<Vector3Int, GameObject> plantedTiles = new Dictionary<Vector3Int, GameObject>();
    public Dictionary<Vector3Int, GameObject> objectTiles = new Dictionary<Vector3Int, GameObject>();
    public Dictionary<Vector3Int, GameObject> treeTiles = new Dictionary<Vector3Int, GameObject>();

    public Dictionary<Vector3Int, GameObject>[] occupiedTiles;

    public bool debugMode;
    public GameObject debugTile;
    public Dictionary<Vector3Int, GameObject> debugTiles = new Dictionary<Vector3Int, GameObject>();

    private void Awake()
    {
        occupiedTiles = new Dictionary<Vector3Int, GameObject>[] { plantedTiles, objectTiles, treeTiles };

        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                dataFromTiles.Add(tile, tileData);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            foreach(KeyValuePair<Vector3Int, bool> entry in wateredTiles.ToList())
            {
                RemoveWaterTile(entry.Key);
            }
        }

        if (debugMode)
        {
            foreach(Dictionary<Vector3Int, GameObject> dictionary in occupiedTiles)
            {
                foreach(KeyValuePair<Vector3Int, GameObject> kp in dictionary)
                {
                    if (!debugTiles.ContainsKey(kp.Key))
                    {
                        var dt = Instantiate(debugTile, kp.Key, Quaternion.identity);
                        debugTiles.Add(kp.Key, dt);
                    }
                }
            }

            foreach(KeyValuePair<Vector3Int, GameObject> debugTile in debugTiles)
            {
                if (CheckForEmptyTile(debugTile.Key)) Destroy(debugTile.Value);
            }
        }
    }

    public bool CheckForEmptyTile(Vector3Int tileLocation)
    {
        foreach(Dictionary<Vector3Int, GameObject> dictionary in occupiedTiles)
        {
            // For each dictionary in occupiedTiles, check the location for an object. If one exists, return false, else true.

            if (dictionary.ContainsKey(tileLocation)) return false;
        }

        return true;
    }

    public void WaterTile(Vector3Int gridPosition)
    {
        if (!wateredTiles.ContainsKey(gridPosition))
        {
            wateredTiles.Add(gridPosition, true);
        }
    }

    public void RemoveWaterTile(Vector3Int gridPosition)
    {
        if (!wateredTiles.ContainsKey(gridPosition)) return;
        wateredLayer.SetTile(gridPosition, null);
        wateredTiles.Remove(gridPosition);
    }

    public void RemoveCrop(Vector3Int gridPosition)
    {
        if (!plantedTiles.ContainsKey(gridPosition)) return;
        GameObject refCrop = plantedTiles[gridPosition];
        plantedTiles.Remove(gridPosition);
        Destroy(refCrop);
    }

    public void RemoveObject(Vector3Int gridPosition)
    {
        if (!objectTiles.ContainsKey(gridPosition)) return;
        GameObject refObject = objectTiles[gridPosition];
        objectTiles.Remove(gridPosition);
        Destroy(refObject);
    }

    public void GrowTiles()
    {
        foreach(KeyValuePair<Vector3Int, GameObject> entry in plantedTiles)
        {
            if (wateredTiles.ContainsKey(entry.Key)) { entry.Value.GetComponent<CropTile>().Grow(); }
        }

        foreach(KeyValuePair<Vector3Int, bool> entry in wateredTiles.ToList())
        {
            RemoveWaterTile(entry.Key);
        }
    }
}
