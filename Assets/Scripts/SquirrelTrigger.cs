using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirrelTrigger : MonoBehaviour
{
    [SerializeField] private float minRandomPosX;
    [SerializeField] private float maxRandomPosX;
    [SerializeField] private float minRandomPosY;
    [SerializeField] private float maxRandomPosY;
    [SerializeField] private float minRandomPosZ;
    [SerializeField] private float maxRandomPosZ;

    [SerializeField] private float speed = 10f;

    [SerializeField] private float triggerDistance = 5f;

    private Animator animator;
    private MyFirstPersonController[] players;

    private Vector3 randomPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        players  = FindObjectsOfType<MyFirstPersonController>();
        randomPosition = ChooseRandomPosition();
    }

    private void Update()
    {
        MyFirstPersonController nearestPlayer = FindNearestPlayer();
        if(Vector3.Distance(transform.position, nearestPlayer.transform.position) < triggerDistance)
        {
            animator.SetBool("jump", true);
            if(!Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 5f))
            {
                
            }
        }
        else
        {
            animator.SetBool("jump", false);
        }
    }

    private MyFirstPersonController FindNearestPlayer()
    {
        MyFirstPersonController nearestPlayer = players[0];
        for (int i = 0, j = 1; j < players.Length; i++, j++)
        {
            float distance1 = Vector3.Distance(transform.position, players[i].transform.position);
            float distance2 = Vector3.Distance(transform.position, players[j].transform.position);

            if(distance1 < distance2)
            {
                nearestPlayer = players[i];
            }
        }

        return nearestPlayer;
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
