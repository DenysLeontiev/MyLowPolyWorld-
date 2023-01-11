using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteText : MonoBehaviour
{
    public void DestroyText()
    {
        GameObject par = gameObject.transform.parent.gameObject; // parent
        Destroy(par);
    }
}
