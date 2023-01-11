using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHitting : MonoBehaviour
{
    [SerializeField] Transform hitArea;
    [SerializeField] float waitForSecondsToHit = 0.8f;

    public static bool hasArm = false; //  cutting with arms
    public static bool hasTool = false;  // cutting with axe
    public static bool hasWeapon = true; //  cutting with weapon(sword etc.)
    bool canHit = true;

    private void Update() 
    {
        // FaceTextMeshToCamera();
        // floatingText.transform.forward = Camera.main.transform.forward;
        //floatingText.transform.rotation = Quaternion.LookRotation(floatingText.transform.position - Camera.main.transform.position );

        if(Input.GetMouseButton(0) && canHit)
        {
            canHit = false;
            PlayerChop();
            StartCoroutine(DelayHitCoroutine());
        }
    }

    private void PlayerChop()
    {
        Vector3 colliderSize = Vector3.one * 0.75f;

        Collider[] colliderArray = Physics.OverlapBox(hitArea.position, colliderSize);
        foreach (Collider collider in colliderArray)
        {
            if(collider.gameObject.TryGetComponent<IDamageable>(out IDamageable treeDamageable))
            {
                int damageAmount = hasArm ? UnityEngine.Random.Range(3,8) :
                                   hasTool ? UnityEngine.Random.Range(9, 15) :
                                   hasWeapon ? UnityEngine.Random.Range(20,30) : 
                                   UnityEngine.Random.Range(3,8);
                // Damage tree
                print(damageAmount);
                treeDamageable.Damage(damageAmount);

                Vector3 colliderPos = collider.transform.position;
                float randXPos = UnityEngine.Random.Range(-0.5f, 0.5f);
                float randYPos = UnityEngine.Random.Range(0.5f, 2f);
                float posZ = (colliderPos.z + transform.position.z) * 0.5f;
                // Vector3 posToSpawn = new Vector3(colliderPos.x + randXPos, colliderPos.y + (Mathf.Abs(collider.transform.GetComponent<Renderer>().bounds.center.y) / collider.transform.lossyScale.y) + 1.3f, posZ);
                // Vector3 posToSpawn = new Vector3(colliderPos.x + randXPos, colliderPos.y + (Mathf.Abs(collider.transform.GetComponentInChildren<Renderer>().bounds.center.y) / collider.transform.lossyScale.y) + 1.3f, posZ);
                Vector3 posToSpawn = new Vector3(colliderPos.x + randXPos, colliderPos.y + 1.3f, posZ);
                Create(posToSpawn, damageAmount);
                //TODO: Add hit visibles and outline of the object + camerashake
                // DamagePopup.Create(transform.position, damageAmount);
                //ShakeCamera
                ShakeCamera(5f, 4f);
                //SpawnFX
            }
        }
    }

    private IEnumerator DelayHitCoroutine()
    {
        yield return new WaitForSeconds(waitForSecondsToHit);
        canHit = true;
    }

    [SerializeField] GameObject floatingText;

    public void Create(Vector3 position, int dmgAmount)
    {
        TextMeshPro textMesh = floatingText.GetComponentInChildren<TextMeshPro>();
        textMesh.color = Color.red;
        textMesh.text = (dmgAmount.ToString());
        Instantiate(floatingText, position, Quaternion.identity);
    }

    private void ShakeCamera(float duration, float magnitude)
    {
        GetComponentInChildren<CameraShake>().ShakeCamera(duration, magnitude);
    }

//     void FaceTextMeshToCamera(){
//             Vector3 origRot = floatingText.transform.eulerAngles;
//         floatingText.transform.LookAt(Camera.main.transform);
//         Vector3 desiredRot = floatingText.transform.eulerAngles;
//         desiredRot.x = origRot.x;
//         desiredRot.z = origRot.z;
//         floatingText.transform.eulerAngles = desiredRot;
//   }
}
