using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellObject : PC_Interactable , IDealDamage
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
        rb.AddForce(transform.up * initialVelocity);
        aliveTime = 0f;

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



        Destroy(gameObject);

    }

    private void AoeDamage(Vector3 impactPoint, float radius, ITakeDamage ignoreTarget = null)
    {

        if (ExplosionParticlePrefab != null)
        {
            GameObject explosion = Instantiate(ExplosionParticlePrefab, impactPoint, Quaternion.identity);
            explosion.transform.localScale *= damageRadius;
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
        rb.AddForce(force);

    }

    public override void Grab(Transform parent)
    {
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        transform.SetParent(parent);
        transform.position = parent.transform.position;
        transform.forward = parent.forward;

    }
}
