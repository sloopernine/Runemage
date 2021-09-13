using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellObject : PC_Interactable , IDealDamage
{
    public float initialLifeTime;
    public float initialVelocity;
    private Rigidbody rb;
    private float aliveTime;
    public float baseDamage;



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
        if (target != null)
        {
            DealDamage(target, baseDamage);
        }
    }

    public void DealDamage(ITakeDamage target, float damage)
    {
        float velocity = rb.velocity.sqrMagnitude;
        if (velocity > 10f)
        {
            target.TakeDamage(baseDamage);
            print($"Dealt {baseDamage} to Target with {velocity} velocity");

        }

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
    }
}
