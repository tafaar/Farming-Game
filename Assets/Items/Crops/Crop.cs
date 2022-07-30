using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_Crop_New", menuName = "Item/ Crop")]
public class Crop : Item
{
    public float sortOffset;

    public int daysToGrow;
    public int currentDays;

    public Sprite[] growthSprites;
    public int[] growthDays;

    public Item harvestedCrop;

    public bool regrows;
    public int regrowDay;
    public Sprite regrowSprite;
}
