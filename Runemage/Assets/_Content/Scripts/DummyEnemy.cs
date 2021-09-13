using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemy : MonoBehaviour, ITakeDamage
{
    public float health;

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 )
        {
            print($"I am Dead with {health} current health");
        }

    }
}
