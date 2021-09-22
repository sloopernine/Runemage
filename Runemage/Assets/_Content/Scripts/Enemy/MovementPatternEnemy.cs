using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPatternEnemy : Enemy
{

    [SerializeField] float minDistanceToPlayer = 4f;
    [SerializeField] float distanceToAttack = 6f;

    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] Vector3 eulerAngleVelocity;

    private void FixedUpdate()
    {
        if (!useMovement)
        {
            return;
        }
        Vector3 playerPosition = Vector3.zero; //Gonna track where the player is later.

        if (DistanceTowardsPoint(playerPosition) < minDistanceToPlayer)
        {
            return;
        }

        //if (DistanceTowardsPoint(playerPosition) < distanceToAttack)
        //{
        //    MoveTowardsPoint(playerPosition);
        //    return;
        //}

        Quaternion deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.fixedDeltaTime * rotationSpeed);
        RotateAndMoveForward(deltaRotation);
        
    }



}
