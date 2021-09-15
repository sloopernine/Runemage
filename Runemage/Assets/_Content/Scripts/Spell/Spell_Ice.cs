using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Ice : SpellObject
{
    public override void Release(Vector3 force)
    {
        transform.parent = null;
        rb.isKinematic = false;
        rb.AddForce(force);
        transform.localScale = new Vector3(0.2f, 0.2f, 3f);
    }

    //public override void Grab(Transform parent)
    //{
    //    rb.velocity = Vector3.zero;
    //    rb.isKinematic = true;
    //    transform.forward = parent.forward;
        
    //    transform.SetParent(parent);
    //}
}
