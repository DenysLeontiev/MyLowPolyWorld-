using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    private Slider slider;

   private void OnEnable()
    {
        MyFirstPersonController.OnDamage += DisplayHealthBar;
        MyFirstPersonController.OnHeal += DisplayHealthBar;
        MyFirstPersonController.OnDamage += DisplayHealthText;
        MyFirstPersonController.OnHeal += DisplayHealthText;
    }

    private void OnDisable()
    {
        MyFirstPersonController.OnDamage -= DisplayHealthBar;
        MyFirstPersonController.OnHeal -= DisplayHealthBar;
        MyFirstPersonController.OnDamage -= DisplayHealthText;
        MyFirstPersonController.OnHeal -= DisplayHealthText;
    }
    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void DisplayHealthBar(float playerHealth)
    {
        slider.value = playerHealth / 100.0f;
    }

    private void DisplayHealthText(float playerHealth)
    {
        GetComponentInChildren<TextMeshProUGUI>().text = Mathf.Clamp(playerHealth, 0, 100).ToString() + "/100";
    }
}
