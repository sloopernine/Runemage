using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singletons;
using Data.Interfaces;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;

public class SpellObject : PC_Interactable , IDealDamage, IReceiveGlobalSignal
{
    [SerializeField] float initialLifeTime;
    [SerializeField] float initialVelocity;
    protected Rigidbody rb;
    protected float aliveTime;
    [SerializeField] float baseDamage;
    [SerializeField] protected float damageRadius;

    [SerializeField] GameObject debugSpherePrefab;

    [SerializeField] GameObject ExplosionParticlePrefab;
    [SerializeField] DamageType damageType;
    [SerializeField] LayerMask layerMask;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        aliveTime = 0f;

        if (GameManager.Instance.usePcInput)
        {
            rb.AddForce(transform.up);
        }
        
        switch (damageType)
        {
            case DamageType.fire:
                GenericSoundController.Instance.Play(WorldSounds.FireballCreate, transform.position);
                break;
            case DamageType.ice:
                GenericSoundController.Instance.Play(WorldSounds.IceSpearCreate, transform.position);
                break;
            default:

                break;
        }

    }

    private void OnEnable()
    {
        GlobalMediator.Instance.Subscribe(this);
    }

    private void OnDisable()
    {
        GlobalMediator.Instance.UnSubscribe(this);
    }

    void Update()
    {
        aliveTime += Time.deltaTime;
        if (aliveTime > initialLifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ITakeDamage target = collision.gameObject.GetComponent<ITakeDamage>();
        if (target == null)
        {
            return;
        }

        DealDamage(target, baseDamage, damageType);

        Vector3 impactPoint = collision.GetContact(0).point;
        AoeDamage(impactPoint, damageRadius, target);

        switch (damageType)
        {
            case DamageType.fire:
                GenericSoundController.Instance.Play(WorldSounds.FireballExplode, impactPoint);
                break;
            case DamageType.ice:
                GenericSoundController.Instance.Play(WorldSounds.IceSpearExplode, impactPoint);
                break;
            default:

                break;
        }

        Destroy(gameObject);

    }

    private void AoeDamage(Vector3 impactPoint, float radius, ITakeDamage ignoreTarget = null)
    {

        if (ExplosionParticlePrefab != null)
        {
            GameObject explosion = Instantiate(ExplosionParticlePrefab, impactPoint, Quaternion.identity);
            //GameObject debug = Instantiate(debugSpherePrefab, impactPoint, Quaternion.identity);
            //debug.transform.localScale *= damageRadius;
        }

        Collider[] Aoe = Physics.OverlapSphere(impactPoint, damageRadius, layerMask);

        foreach (Collider hit in Aoe)
        {
            ITakeDamage secondaryTarget = hit.gameObject.GetComponent<ITakeDamage>();
            if (secondaryTarget == null || secondaryTarget == ignoreTarget)
            {
                continue;
            }

            DealDamage(secondaryTarget, baseDamage / 2f, damageType);
        }
    }

    public void DealDamage(ITakeDamage target, float damage, DamageType damageType)
    {
        target.TakeDamage(damage, damageType);        
        print($"{gameObject.name} Dealt {damage} to Target");
       
    }

    public override void Release(Vector3 force)
    {
        transform.parent = null;
        rb.isKinematic = false;
        //NO! rb.AddForce(force);
        TurnOnGravity();
    }

    public override void Grab(Transform parent)
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        transform.SetParent(parent);
        transform.position = parent.transform.position;
        transform.forward = parent.forward;
    }

    public void TurnOnGravity()
    {
        rb.useGravity = true;
    }

    public void SetKinematic(bool setValue)
    {
        rb.isKinematic = setValue;
    }

    public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {
        switch (eventState)
        {
            case GlobalEvent.SPELLS_DESTROY_ALL:
                Destroy(gameObject);
                break;
        }
    }
}
