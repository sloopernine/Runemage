using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellConjuror : MonoBehaviour
{

    public Spell currentSpell;
    public GameObject spellBallPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateSpell();

        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwapSpell();

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ConjureSpell();

        }


        if (Input.GetKeyDown(KeyCode.R))
        {

            ToggleHoldSpell();
        }

    }

    private void ToggleHoldSpell()
    {
        SpellCastOrigin spellCastOrigin = TryToFindSpell();
        if (spellCastOrigin == null)
        {
            return;
        }

        if (spellCastOrigin.transform.IsChildOf(transform))
        {
            spellCastOrigin.transform.parent = null;
            return;
        }

        spellCastOrigin.transform.SetParent(transform);

    }

    private void ActivateSpell()
    {
        SpellCastOrigin spellCastOrigin = TryToFindSpell();
        if (spellCastOrigin == null)
        {
            return;
        }

        spellCastOrigin.CastSpell(transform.position);
    }

    public SpellCastOrigin TryToFindSpell()
    {
        //Find spell with ray
        RaycastHit hit;
        SpellCastOrigin spellCastOrigin = null;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000f))
        {
            Debug.DrawLine(transform.position, transform.forward * 1000f, Color.red, 3f);

            spellCastOrigin = hit.transform.GetComponent<SpellCastOrigin>();

        }

        return spellCastOrigin;
       
    }

    private void SwapSpell()
    {
        currentSpell++;
        int amountOfSpells = Enum.GetValues(typeof(Spell)).Length;
        int spellIndex = (int)currentSpell;
        spellIndex = spellIndex % amountOfSpells;
        currentSpell = (Spell)spellIndex;

        print(Enum.GetName(typeof(Spell), currentSpell) + " is ready");
    }

    private void ConjureSpell()
    {

        SpellCastOrigin spellCastOrigin = Instantiate(spellBallPrefab).GetComponent<SpellCastOrigin>();
        spellCastOrigin.currentSpell = currentSpell;
        spellCastOrigin.transform.position = transform.position + transform.forward;
        spellCastOrigin.transform.LookAt(transform.position);

    }




}
