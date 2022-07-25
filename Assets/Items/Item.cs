using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item_New", menuName = "Item/ Item")]
public class Item : ScriptableObject
{
    public enum Type { ITEM, TOOL, FRUIT, VEGETABLE }

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public string name;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    public Type type;
    public Sprite icon;
    public Sprite overworldSprite;
    [TextArea(4,4)]
    public string description;
    public int maxStack;
    public int currentStack = 1;
    public bool edible;
    public int health;
    public int energy;
    public bool placeable;
    public bool canBeDropped = true;
    public bool canBeSold;
    public int basePrice;

    

}
