using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, ITakeDamage
{
    private Rigidbody rigidbody;

    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    [SerializeField] float speed;

    [SerializeField] AudioClip spawnSound;
    [SerializeField] AudioClip deathSound;
    private AudioSource audioSource;
    private float addForcePower = 140;

    public bool useMovement;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(spawnSound);

        currentHealth = maxHealth;

        if (!useMovement)
        {
            rigidbody.AddForce(transform.forward * speed * addForcePower);
        }
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <=0 )
        {
            Die();
        }
    }

    public void Die()
    {
        audioSource.PlayOneShot(deathSound);
        Destroy(gameObject);
    }

    public void MoveTowardsPoint(Vector3 point)
    { 
        Vector3 direction = point - transform.position;
        direction = direction.normalized;
        rigidbody.MovePosition(transform.position + direction * Time.deltaTime * speed);
    }

    public float DistanceTowardsPoint(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        float distance = direction.magnitude;

        return distance;
    }
}
