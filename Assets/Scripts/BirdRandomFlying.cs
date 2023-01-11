using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdRandomFlying : MonoBehaviour
{
    [SerializeField] private float minRandomPosX;
    [SerializeField] private float maxRandomPosX;
    [SerializeField] private float minRandomPosY;
    [SerializeField] private float maxRandomPosY;
    [SerializeField] private float minRandomPosZ;
    [SerializeField] private float maxRandomPosZ;

    [SerializeField] private float speed = 10f;

    private Vector3 randomPosition;
    

    void Start()
    {
        randomPosition = ChooseRandomPosition();
    }

    void Update()
    {
        Fly();
    }

    private void Fly()
    {
        if (Physics.Raycast(transform.position, randomPosition, out RaycastHit hit, 15f))
        {
            randomPosition = ChooseRandomPosition();
        }
        transform.LookAt(randomPosition);
        Vector3 currentPos = transform.position;
        transform.position = Vector3.MoveTowards(currentPos, randomPosition, speed * Time.deltaTime);
        if (Vector3.Distance(currentPos, randomPosition) <= 3f)
        {
            randomPosition = ChooseRandomPosition();
        }
    }

    private Vector3 ChooseRandomPosition()
    {
        float randX = UnityEngine.Random.Range(minRandomPosX, maxRandomPosX);
        float randY = UnityEngine.Random.Range(minRandomPosY, maxRandomPosY);
        float randZ = UnityEngine.Random.Range(minRandomPosZ, maxRandomPosZ);

        Vector3 randVector = new Vector3(randX, randY, randZ);

        return randVector;
    }
}
