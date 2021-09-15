using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Interfaces;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Singletons;

public abstract class Enemy : MonoBehaviour, ITakeDamage, ISendGlobalSignal
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
        BasicData data = new BasicData(gameObject.tag);
        SendGlobal(GlobalEvent.OBJECT_INACTIVE, data); //We need to might att what gameobject it is!
        gameObject.SetActive(false);
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

    public void SendGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {
        GlobalMediator.Instance.ReceiveGlobal(eventState, globalSignalData);
    }

    //This would be based on a tag in the future when I dare to change the tag settings in Unity.
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Death Zoone")
        {
            Debug.Log("Killed Enemy. It was out of the arena");
            Die();
        }
    }
}
