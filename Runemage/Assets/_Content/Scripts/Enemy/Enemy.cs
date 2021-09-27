using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Interfaces;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Singletons;
using System;

public abstract class Enemy : MonoBehaviour, ITakeDamage, IDealDamage, ISendGlobalSignal
{

    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;

    [Header("Freeze Settings")]
    [Tooltip("Per cent of the speed value")]
    [Range(0,1)]
    [SerializeField] float freezedSpeed;
    [SerializeField] float damage;
    [SerializeField] float freezedTime;
    public bool isFreezed;

    public bool isAttacking;
    private EnemyMovement enemyMovement;

    [SerializeField] Animator animator;

    private void Start()
    {

        enemyMovement = GetComponent<EnemyMovement>();
       
        if (animator == null)
        {
            print($"No animator Set for {transform.name}");
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
        animator.SetTrigger("Damage");

        if (damageType == DamageType.ice)
        {
            Freeze();
        }

    }


    public void DealDamage(ITakeDamage target, float damage, DamageType damageType)
    {
        if (isAttacking)
        {
            animator.SetTrigger("Attack");

            target.TakeDamage(damage, DamageType.enemy);
        }
    }

    public void Die()
    {
        StopAllCoroutines();
        if (GenericSoundController.Instance != null)
        {
            GenericSoundController.Instance.Play(WorldSounds.EnemyDeath, transform.position);
        }

        BasicData data = new BasicData(gameObject.tag);
        SendGlobal(GlobalEvent.OBJECT_INACTIVE, data); //We need to might att what gameobject it is!
        gameObject.SetActive(false);
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
        enemyMovement.SetCurrentSpeed(enemyMovement.CurrentSpeed * freezedSpeed);

        yield return new WaitForSeconds(freezedTime);
        enemyMovement.SetCurrentSpeed(enemyMovement.InitialSpeed);

        isFreezed = false;
    }

    public void OnSpawn()
    {
        //Debug.Log("Me is enabled");
        StopAllCoroutines();       
        currentHealth = maxHealth;
        isFreezed = false;

        if (GenericSoundController.Instance != null)
        {
            GenericSoundController.Instance.Play(WorldSounds.EnemySpawn, transform.position);
        }
    }
}
