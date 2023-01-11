using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator ShakeCamera(float duration,float magnitude)
    {
        Vector3 initialPosition = transform.localPosition;

        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            float xShakePos = Random.Range(-1,1) * magnitude;
            float yShakePos = Random.Range(-1,1) * magnitude;

            transform.localPosition = new Vector3(xShakePos, yShakePos, initialPosition.z);

            elapsed += Time.deltaTime;

        yield return  null; // Waits for the end of frame
        }

        transform.localPosition = initialPosition;
    }
}
