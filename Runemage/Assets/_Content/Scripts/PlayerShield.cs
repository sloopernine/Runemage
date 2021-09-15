using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour, ITakeDamage
{
    public float maxHealth = 200;
    public float currentHealth;
    public float armor = 1000;
    private float reducedDamage;

    private void Start()
    {
        currentHealth = maxHealth;
        reducedDamage = armor / 100;
    }

    public void TakeDamage(float damage, DamageType damageType)
    {
        currentHealth -= damage * reducedDamage;
        Debug.Log(damageType);
    }
}
