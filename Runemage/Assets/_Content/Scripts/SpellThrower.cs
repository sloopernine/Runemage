using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellThrower : MonoBehaviour
{
    
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GrabSpell();

        }
    }

    private void GrabSpell()
    {
        PC_Interactable interactable = TryToFindInteractable();
        if (interactable == null)
        {
            return;
        }

        if (interactable.transform.IsChildOf(transform))
        {
            interactable.transform.parent = null;
            return;
        }

        interactable.transform.SetParent(transform);
    }

    public PC_Interactable TryToFindInteractable()
    {
        //Find spell with ray
        RaycastHit hit;
        PC_Interactable interactable = null;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000f))
        {
            Debug.DrawLine(transform.position, transform.forward * 1000f, Color.red, 3f);

            interactable = hit.transform.GetComponent<PC_Interactable>();

        }

        return interactable;

    }
}
