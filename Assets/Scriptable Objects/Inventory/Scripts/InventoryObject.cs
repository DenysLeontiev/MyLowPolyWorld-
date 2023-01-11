using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Net.Mime;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string filePath;
    public ItemDatabaseObject databse;
    public Inventory Container;
    // private ItemDatabaseObject databse;

//     private void OnEnable()
//     {
// #if UNITY_EDITOR
//         databse = (ItemDatabaseObject)AssetDatabase.LoadAssetAtPath("Assets/Resources/Scriptable Objects/Items/Database.asset", typeof(ItemDatabaseObject));
// #else
//         databse = Resources.Load<ItemDatabaseObject>("Database");
// #endif
//     }

    
    public bool AddItem(Item _item, int _amount)
    {
        if(EmptySlotCount <= 0)
        {
            return false;
        }
        InventorySlot slot = FindItemOnInvetory(_item);
        if(!databse.Items[_item.Id].stackable || slot == null)
        {
            SetEmptySlot(_item, _amount);
            return true; 
        }

        slot.AddAmount(_amount);
        return true;
    }

    private InventorySlot FindItemOnInvetory(Item _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if(Container.Items[i].item.Id == _item.Id)
            {
                return Container.Items[i];
            }
        }

        return null;
    }

    public int EmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < Container.Items.Length; i++)
            {
                if(Container.Items[i].item.Id <= -1)
                {
                    counter++;
                }
            }
            return counter;
        }
    }

    public InventorySlot SetEmptySlot(Item _item, int _amount)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if(Container.Items[i].item.Id <= -1)
            {
                Container.Items[i].UpdateSlot(_item, _amount);
                return Container.Items[i];
            }
        }
        // set up functionality when your inventory is full
        return null;
    }

    public void SwapItems(InventorySlot item1, InventorySlot item2)
    {
        if(item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject))
        {
            InventorySlot temp = new InventorySlot(item2.item, item2.amount);
            item2.UpdateSlot(item1.item, item1.amount);
            item1.UpdateSlot(temp.item, temp.amount);
        }
        
    }

    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if(Container.Items[i].item == _item)
            {
                Container.Items[i].UpdateSlot( null, 0);
            }
        }
    }

    [ContextMenu("Save")]
    public void Save() // Use IFormatter because Json allows to changee files whereas IFormater doesn`t
    {
        // var saveData = JsonUtility.ToJson(this, true);
        // BinaryFormatter binaryFormatter = new BinaryFormatter();
        // using FileStream file = File.Create(string.Concat(Application.persistentDataPath, filePath));
        // binaryFormatter.Serialize(file, saveData);
        IFormatter formatter = new BinaryFormatter();
        using Stream stream = new FileStream(string.Concat(Application.persistentDataPath, filePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if(File.Exists(string.Concat(Application.persistentDataPath, filePath)))
        {
            // BinaryFormatter binaryFormatter = new BinaryFormatter();
            // using FileStream file = File.Open(string.Concat(Application.persistentDataPath, filePath), FileMode.Open);
            // JsonUtility.FromJsonOverwrite(binaryFormatter.Deserialize(file).ToString(), this);
            IFormatter formatter = new BinaryFormatter();
            using var stream = new FileStream(string.Concat(Application.persistentDataPath, filePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = formatter.Deserialize(stream) as Inventory;
            for (int i = 0; i < Container.Items.Length; i++)
            {
                Container.Items[i].UpdateSlot(newContainer.Items[i].item, newContainer.Items[i].amount);
            }
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();
    }

    // public void OnAfterDeserialize() // when scriptable object is changed,unity auto. serializes it again; 
    // {
    //     for (int i = 0; i < Container.Items.Count; i++)
    //     {
    //         Container.Items[i].item = databse.GetItem[Container.Items[i].ID];
    //     }
    // }

    // public void OnBeforeSerialize()
    // {

    // }
}

[Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[28];
    public void Clear()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].RemoveItem();
        }
    }
}


[Serializable]
public class InventorySlot
{
    public Type[] AllowedItems = new Type[0];
    [NonSerialized]
    public UserInterface parent;

    public Item item;
    public int amount;

    public ItemObjects ItemObject
    {
        get
        {
            if(item.Id > 0)
            {
                return parent.inventory.databse.Items[item.Id];
            }
            return null;
        }
    }

    public InventorySlot(Item _item, int _amount)
    {
        item = _item;
        amount = _amount;   
    }

    public InventorySlot()
    {
        item = new Item();
        amount = 0;   
    }

    public void UpdateSlot(Item _item, int _amount)
    {
        item = _item;
        amount = _amount; 
    }

    public void RemoveItem()
    {
        item = new Item();
        amount = 0;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }

    public bool CanPlaceInSlot(ItemObjects _itemObject)
    {
        if(AllowedItems.Length <= 0 || _itemObject == null || _itemObject.data.Id < 0)
        {
            return true;
        }
        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if(_itemObject.type == AllowedItems[i])
            {
                return true;
            }
        }

        return false;
    }
}
