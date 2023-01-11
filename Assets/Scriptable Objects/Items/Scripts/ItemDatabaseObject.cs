using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObjects[] Items;
    //public Dictionary<int, ItemObjects> GetItem = new Dictionary<int, ItemObjects>();

    [ContextMenu("Update ID's")]
    public void UpdateId()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if(Items[i].data.Id != i)
            {
                Items[i].data.Id = i;
            }
        }
    }

    public void OnAfterDeserialize()
    {
        UpdateId();
        // for (int i = 0; i < Items.Length; i++)
        // {
        //     Items[i].data.Id = i;
        //     //GetItem.Add(i, Items[i]);
        // }
    }

    public void OnBeforeSerialize()
    {
        //GetItem = new Dictionary<int, ItemObjects>();
    } 
}
