using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HitObjectHealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private AudioSource hitObjectSound;
    [SerializeField] AudioSource dieObjectSound;

    private TextMeshProUGUI healthText;
    [SerializeField] private Slider sliderValue;


    private int health = 100;
    private bool canSpawn = false;

    private int maxHealth;
    private CanvasScaler canvasScaler; // To make slider to look at player (it's only purpose)
    private Camera playerCamera;

    private Image fadeImage;

    private void Start()
    {
        maxHealth = health;
        healthText = GetComponentInChildren<TextMeshProUGUI>();
        canvasScaler = GetComponentInChildren<CanvasScaler>();
        playerCamera = Camera.main;
        fadeImage = GetComponentInChildren<Slider>().GetComponentInChildren<Image>();
    }

    void Update()
    {
        HandleObjectDeath();
        DisplayHealth();
        canvasScaler.transform.forward = playerCamera.transform.forward;

        // if(Physics.Raycast(playerCamera.transform.position, playerCamera.transform.TransformDirection(Vector3.forward), out RaycastHit hit, 6f))
        // {           
        //     if(hit.transform.TryGetComponent<IDamageable>(out IDamageable damageable))
        //     {
        //         float distanceBetween = Vector3.Distance(playerCamera.transform.position, transform.position);
        //         float mappedValue = map(distanceBetween,6.0f, 0.0f,0.0f,1.0f);
        //         fadeImage.color = new Color(fadeImage.color.r,fadeImage.color.g,fadeImage.color.b,mappedValue);
        //         DisplayHealth();
        //         canvasScaler.transform.position = transform.position + 2 * Vector3.up;
        //         canvasScaler.transform.forward = playerCamera.transform.forward;
        //     }
        //     else
        //     {
        //     }
        // }
    }

    private void HandleObjectDeath()
    {
        if (health <= 0 && !canSpawn)
        {
            dieObjectSound.Play();
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
            canSpawn = true;
            Destroy(this.gameObject, 0.4f);
        }
    }


    public void Damage(int damage)
    {
        hitObjectSound.Play();
        health -= damage;
    }

    private void DisplayHealth()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        healthText.text = health.ToString();
        sliderValue.value = health / 100f;
    }

    public float map(float value, float leftMin, float leftMax, float rightMin, float rightMax )
    {
        return rightMin + ( value - leftMin ) * ( rightMax - rightMin ) / ( leftMax - leftMin );
    }
}
