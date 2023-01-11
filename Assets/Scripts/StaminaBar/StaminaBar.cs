using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    private Slider slider;
    private TextMeshProUGUI textToDisplayStamina;

    private void OnEnable()
    {
        MyFirstPersonController.OnReduceStamina += DisplayStamina;
        MyFirstPersonController.OnRenewStamina += DisplayStamina;
    }

    private void OnDisable()
    {
        MyFirstPersonController.OnReduceStamina -= DisplayStamina;
        MyFirstPersonController.OnRenewStamina -= DisplayStamina;
    }

    void Start()
    {
        slider = GetComponent<Slider>();
        textToDisplayStamina = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        
    }

    private void DisplayStamina(float playerStamina)
    {
        textToDisplayStamina.text = $"{(int)playerStamina}/100";
        slider.value = playerStamina / 100;
    }
}
