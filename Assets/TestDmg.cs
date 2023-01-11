using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDmg : MonoBehaviour
{
   void OnTriggerEnter(Collider other)
   {
        if(other.tag == "Player")
        {
            MyFirstPersonController.OnDamageTaken(15);
            print("i am in");
        }
   }
}
