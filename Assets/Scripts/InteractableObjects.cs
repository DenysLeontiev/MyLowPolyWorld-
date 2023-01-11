using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObjects : Interactable
{
    [SerializeField] Transform placeToSpawn;

    public static bool isArmedMelee = false;
    
    public override void OnFocus()
    {
        print("LOOKING AT " + gameObject.name);
    }

    private GameObject currentWeapon = null;
    public override void OnInteract()
    {

        // if(gameObject.tag == "Sword")
        // {
        //     if(currentWeapon != null)
        //     {
        //         currentWeapon.transform.parent = null;
        //         // Destroy(currentWeapon);
        //         currentWeapon = gameObject;
        //     }
        //     else
        //     {
        //         currentWeapon = gameObject;
        //     }

        //     currentWeapon.transform.parent = GameObject.Find("ToolHandler").transform;
        //     currentWeapon.transform.localPosition = new Vector3(0.094f, 0.078f, 0);
        //     currentWeapon.transform.localRotation = Quaternion.Euler(-26.975f, 188.719f, -14.236f);
        //     isArmedMelee = true;
        // }
        // if(gameObject.tag == "Axe")
        // {
        //     if(currentWeapon != null)
        //     {
        //         currentWeapon.transform.parent = null;
        //         // Destroy(currentWeapon);
        //         currentWeapon = gameObject;
        //     }
        //     else
        //     {
        //         currentWeapon = gameObject;
        //     }

        //     currentWeapon.transform.parent = GameObject.Find("ToolHandler").transform;
        //     currentWeapon.transform.localPosition = new Vector3(0.094f, 0.078f, 0);
        //     currentWeapon.transform.localRotation = Quaternion.Euler(-26.975f, 188.719f, -14.236f);
        //     isArmedMelee = true;
        // }
        
    }

    public override void OnLoseFocus()
    {

    }
}
