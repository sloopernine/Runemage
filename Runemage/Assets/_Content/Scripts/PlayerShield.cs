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
    private MeshRenderer meshRenderer;
    private float reducedDamage;
    private float hitDuration = 0.05f;

    private bool isBroken;
    public bool getIsBroken { get { return isBroken; } }

    private void Start()
    {
        currentHealth = maxHealth;
        reducedDamage = armor / 100;
        sheildInfoText.enabled = false;
        meshRenderer = GetComponentInChildren<MeshRenderer>();
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
        HitEffect();
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

    private void HitEffect()
    {
        Material material = meshRenderer.material;
        StartCoroutine(FrenselEffect(material, hitDuration));
    }

    private IEnumerator FrenselEffect(Material material, float time)
    {
        //Make frensel effect go op on hit
        yield return new WaitForSeconds(time);
        //Make the frensel effect go back to normal
    }
}
