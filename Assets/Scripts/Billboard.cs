using System.Net.Mime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera playerCamera;
    [SerializeField] TextMeshProUGUI interactionText;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerCamera = Camera.main;
    }
    void LateUpdate() //  бо про всяк випадок,чекаємо поки гравіка прогрузиться
    {
        float distanceBetween = Vector3.Distance(playerCamera.transform.position, transform.position);
        float mappedValue = map(distanceBetween,6.0f, 0.0f,0.0f,1.0f);
        spriteRenderer.color = new Color(spriteRenderer.color.r,spriteRenderer.color.g,spriteRenderer.color.b,mappedValue);
        interactionText.color = new Color(interactionText.color.r,interactionText.color.g,interactionText.color.b,mappedValue);

        transform.forward = playerCamera.transform.forward;
    }

    // Maps a value from ome arbitrary range to another arbitrary range
    public float map(float value, float leftMin, float leftMax, float rightMin, float rightMax )
    {
        return rightMin + ( value - leftMin ) * ( rightMax - rightMin ) / ( leftMax - leftMin );
    }
}
