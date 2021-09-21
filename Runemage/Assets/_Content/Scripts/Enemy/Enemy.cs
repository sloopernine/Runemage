using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Interfaces;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Singletons;

public abstract class Enemy : MonoBehaviour, ITakeDamage, IDealDamage, ISendGlobalSignal
{
    private Rigidbody rigidbody;

    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    [SerializeField] float speed;
    private float currentSpeed;

    [Header("Freez Settings")]
    [Tooltip("Procent of the speed value")]
    [Range(0,1)]
    [SerializeField] float freezedSpeed;
    [SerializeField] float damage;
    [SerializeField] float freezedTime;
    public bool isFreezed;

    public bool isAttacking;

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

        currentSpeed = speed;

        if (!useMovement)
        {
            rigidbody.AddForce(transform.forward * currentSpeed * addForcePower);
        }
    }

    public void TakeDamage(float damage, DamageType damageType)
    {
        currentHealth -= damage;
        if (currentHealth <=0 )
        {
            Die();
            return;
        }

        if (damageType == DamageType.ice)
        {
            Freeze();
        }
    }

    public void DealDamage(ITakeDamage target, float damage, DamageType damageType)
    {
        if (isAttacking)
        {
            target.TakeDamage(damage, DamageType.enemy);
        }
    }

    public void Die()
    {
        StopAllCoroutines();
        //audioSource.PlayOneShot(deathSound);
        BasicData data = new BasicData(gameObject.tag);
        SendGlobal(GlobalEvent.OBJECT_INACTIVE, data); //We need to might att what gameobject it is!
        gameObject.SetActive(false);
    }

    public void MoveTowardsPoint(Vector3 point)
    { 
        Vector3 direction = point - transform.position;
        direction = direction.normalized;
        rigidbody.MovePosition(transform.position + direction * Time.deltaTime * currentSpeed);
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

    public void OnTriggerEnter(Collider other)
    {
        ITakeDamage target = other.gameObject.GetComponent<ITakeDamage>();
        LayerMask mask = other.gameObject.layer;
        if (target != null && mask == 9)
        {
            DealDamage(target, damage, DamageType.enemy);
        }
    }

    private void Freeze()
    {
        StartCoroutine(FreezTimer(freezedTime));
    }

    private IEnumerator FreezTimer(float freezedTime)
    {
        isFreezed = true;
        currentSpeed = speed * freezedSpeed;
        yield return new WaitForSeconds(freezedTime);
        currentSpeed = speed;
        isFreezed = false;
    }

    private void OnEnable()
    {
        //Debug.Log("Me is enabled");
        currentHealth = maxHealth;
        currentSpeed = speed;
        isFreezed = false;

        if (!useMovement)
        {
            rigidbody.AddForce(transform.forward * currentSpeed * addForcePower);
        }
    }
}
