using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellThrower : MonoBehaviour
{

    [SerializeField] float pC_ThrowSpeed;
    [SerializeField] float grabRange = 10f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleGrabSpell();
            print(grabRange);

        }

    }

    private void ToggleGrabSpell()
    {
        PC_Interactable interactable = TryToFindInteractable();
        if (interactable == null)
        {
            return;
        }

        if (interactable.transform.IsChildOf(transform))
        {
            interactable.transform.parent = null;
            interactable.Release(transform.forward * pC_ThrowSpeed);
            return;
        }

        interactable.Grab(transform);
    }

    public PC_Interactable TryToFindInteractable()
    {
        //Find spell with ray
        RaycastHit hit;
        PC_Interactable interactable = null;

        if (Physics.Raycast(transform.position, transform.forward, out hit, grabRange))
        {
            Debug.DrawLine(transform.position, transform.position + transform.forward * grabRange, Color.red, 1f);

            interactable = hit.transform.GetComponent<PC_Interactable>();

        }

        return interactable;

    }
}
