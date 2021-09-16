using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerShield : MonoBehaviour, ITakeDamage
{
    [SerializeField] float maxHealth = 200;
    [SerializeField] float currentHealth;
    [SerializeField] float armor = 1000;
    [SerializeField] TextMeshProUGUI sheildInfoText;
    private float reducedDamage;
    private MeshRenderer meshRenderer;
    private bool isBroken;
    public bool getIsBroken { get { return isBroken; } }

    private void Start()
    {
        currentHealth = maxHealth;
        reducedDamage = armor / 100;
        meshRenderer = GetComponent<MeshRenderer>();
        sheildInfoText.enabled = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.N) && isBroken)
        {
            RebuildShield();
        }
    }

    public void TakeDamage(float damage, DamageType damageType)
    {
        currentHealth -= damage * reducedDamage;
        if (currentHealth <= 0)
        {
            BreakSheild();
        }
    }

    private void BreakSheild()
    {
        sheildInfoText.enabled = true;
        meshRenderer.enabled = false;
        isBroken = true;
    }

    private void RebuildShield()
    {
        sheildInfoText.enabled = false;
        meshRenderer.enabled = true;
        currentHealth = maxHealth;
        isBroken = false;
    }
}
