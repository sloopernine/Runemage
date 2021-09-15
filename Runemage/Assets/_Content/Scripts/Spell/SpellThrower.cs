using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellThrower : MonoBehaviour
{

    [SerializeField] float pC_ThrowSpeed;
    [SerializeField] float grabRange = 10f;

    GameObject pc_Hand;

    private void Start()
    {
        pc_Hand = new GameObject("pc_Hand");
        pc_Hand.transform.position = transform.position + transform.forward;
        pc_Hand.transform.forward = transform.forward;
        pc_Hand.transform.SetParent(transform);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleGrabSpell();
            //print(grabRange);

        }

    }

    private void ToggleGrabSpell()
    {
        PC_Interactable interactable = TryToFindInteractable();
        if (interactable == null)
        {
            return;
        }

        if (interactable.transform.IsChildOf(pc_Hand.transform))
        {
            interactable.Release(transform.forward * pC_ThrowSpeed);
            return;
        }

        interactable.Grab(pc_Hand.transform);

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
