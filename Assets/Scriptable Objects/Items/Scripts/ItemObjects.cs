using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Type
{
    Tools,
    Weapons,
    Food, 
    Resources,
    Default,
    Helmet,
    Shield,
    Boots,
    Chest
}

public enum Attributes
{
    Speed,
    JumpForce,
    Stamina,
    Strength,
    DoesNotStack
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/item")]
public class ItemObjects : ScriptableObject
{
    public Sprite uiDisplay;  //image to display in inventory
    public bool stackable;
    public Type type;
    [TextArea(15,20)]
    public string description;
    public Item data = new Item();

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }
}

[Serializable]
public class Item
{
    public string Name;
    public int Id = -1;
    public ItemBuff[] buffs;
    public Item()
    {
        Name = "";
        Id = -1;
    }
    public Item(ItemObjects item)
    {
        Name = item.name;
        Id = item.data.Id;
        buffs = new ItemBuff[item.data.buffs.Length];  // because array is a reference type
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(item.data.buffs[i].min, item.data.buffs[i].max)
            {
                attributes = item.data.buffs[i].attributes,
            };
            // buffs[i].attributes = item.buffs[i].attributes;
        }
    }
}

[Serializable]
public class ItemBuff
{
    public Attributes attributes;
    public int value;
    public int min;
    public int max;
    public ItemBuff(int _min, int _max)
    {
        min = _min;
        max = _max;
        GenerateValue();
    }
    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}
