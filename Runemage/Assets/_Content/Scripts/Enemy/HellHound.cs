using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellHound : Enemy
{
    [SerializeField] float minDistanceToPlayer = 4f;

    private void FixedUpdate()
    {
        //if (useMovement)
        //{
        //    Vector3 playerPosition = Vector3.zero; //Gonna track where the player is later.
            
        //    if(DistanceTowardsPoint(playerPosition) > minDistanceToPlayer)
        //    {
        //        MoveTowardsPoint(playerPosition);
        //    }         
        //}
    }
}
