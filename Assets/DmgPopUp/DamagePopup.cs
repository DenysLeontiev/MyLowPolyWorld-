// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;

// public class DamagePopup : MonoBehaviour
// {
//     public static DamagePopup Create(Vector3 position, int damageAmount)
//     {
//         Transform dmgPopup = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);
//         DamagePopup damagePopup = dmgPopup.GetComponent<DamagePopup>();
//         damagePopup.SetUp(damageAmount);

//         return damagePopup;
//     }

//     public const float DISAPPEAR_TIMER_MAX = 1f;

//     private TextMeshPro textMesh;
//     private float disaaperTimer;
//     private Color textColor;

//     private void Awake()
//     {
//         textMesh = GetComponent<TextMeshPro>(); 
//     }

//     public void SetUp(int damageAmount)
//     {
//         textMesh.SetText(damageAmount.ToString());
//         textColor = textMesh.color;
//         disaaperTimer = DISAPPEAR_TIMER_MAX;
//     }

//     private void Update()
//     {
//         float moveYSpeed = 20f;
//         transform.position += new Vector3(0f, (moveYSpeed) * Time.deltaTime, 0f);

//         if(disaaperTimer > DISAPPEAR_TIMER_MAX * 0.5f)
//         {
//             float increaseScaleAmount = 1f;
//             transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
//         }
//         else
//         {
//             float decreaseScaleAmount = 1f;
//             transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
//         }

//         disaaperTimer -= Time.deltaTime;
//         if(disaaperTimer < 0)
//         {
//             float disapperSpeed = 3f;
//             textColor.a -= disapperSpeed * Time.deltaTime;
//             textMesh.color = textColor;
            
//             if(textColor.a < 0)
//             {
//                 Destroy(gameObject);
//             }
//         }
//     }
// }
