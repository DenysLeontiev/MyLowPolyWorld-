using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleInventoryShowHide : MonoBehaviour
{
    [SerializeField] private KeyCode inventoryKey = KeyCode.Q;
    [SerializeField] private GameObject inventoryCanvas;
    bool isInventoryActive = false;

    private void Start()
    {
        inventoryCanvas.SetActive(false);
    }

    private void Update()
    {
        HandleInventory();
    }

    private void HandleInventory()
    {
        if(Input.GetKeyDown(inventoryKey))
        {
            isInventoryActive = !isInventoryActive;
        }

        if(isInventoryActive)
        {
            inventoryCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            inventoryCanvas.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        }
    }
}
