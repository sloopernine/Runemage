using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellHound : Enemy
{
    [SerializeField] float minDistanceToPlayer = 4f;

    private void FixedUpdate()
    {
        if (useMovement)
        {
            Vector3 playerPosition = Vector3.zero; //Gonna track where the player is later.
            
            if(DistanceTowardsPoint(playerPosition) > minDistanceToPlayer)
            {
                MoveTowardsPoint(playerPosition);
            }         
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        //Check if it is a damage spell
        if (collision.gameObject.tag != "Enemy")
        {
            TakeDamage(50);

            //Maybe be able to destroy that object if its is a one hit spell?
            //Destroy(collision.gameObject);
        }
    }
}
