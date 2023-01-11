using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextLookAtPlayer : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        // transform.forward = Camera.main.transform.forward;
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position );
    }
}
