using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Singletons;
using Data.Interfaces;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;

public class PlayerShield : MonoBehaviour, ITakeDamage, IReceiveGlobalSignal
{
    [SerializeField] float maxHealth = 200;
    [SerializeField] float currentHealth;
    [SerializeField] float armor = 1000;
    [SerializeField] TextMeshProUGUI sheildInfoText;
    private MeshRenderer meshRenderer;

    private float reducedDamage;

    //Shader effect values
    private float delay = 0.05f;
    private float frenselPower = 4;
    private float frenselChange = 0.05f;
    private string frenselProperty = "Frensel_Power";
    private string brokenProperty = "Broken_Value";
    private string brokenPointsProperty = "Broken_Points";

    private bool isBroken;
    public bool getIsBroken { get { return isBroken; } }

    [SerializeField] bool isInvulnerable;

    private void Start()
    {
        if (maxHealth == 0)
        {
            maxHealth = 0.0001f;
        }
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
        if (isInvulnerable)
        {
            return;
        }

        currentHealth -= damage * reducedDamage;
        HitEffect();
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

    private void OnEnable()
    {
        GlobalMediator.Instance.Subscribe(this);
    }

    private void OnDisable()
    {
        GlobalMediator.Instance.UnSubscribe(this);

    }

    public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {
        switch (eventState)
        {
            case GlobalEvent.SHIELD_INVULNERABLE_ON:
                SetIsInvulnerable(true);
                break;
            case GlobalEvent.SHIELD_INVULNERABLE_OFF:
                SetIsInvulnerable(false);
                break;

        }
    }

    private void SetIsInvulnerable(bool value)
    {
        isInvulnerable = value;
    private void HitEffect()
    {
        StopAllCoroutines();
        Material material = meshRenderer.material;
        StartCoroutine(FrenselEffect(material, 3));
        StartCoroutine(BrokenEffect(material, currentHealth));
    }

    private IEnumerator FrenselEffect(Material material, float minFrensel)
    {
        float tempFrensel = frenselPower;
        while (tempFrensel > minFrensel)
        {
            yield return new WaitForSeconds(delay);

            tempFrensel -= frenselChange;
            material.SetFloat(frenselProperty, tempFrensel);

        }
        material.SetFloat(frenselProperty, frenselPower);
    }

    private IEnumerator BrokenEffect(Material material, float health)
    {
        yield return new WaitForSeconds(delay);
        float brokenValue = (currentHealth / maxHealth);
        brokenValue = 1 - brokenValue;
        float brokenPoints = (maxHealth - currentHealth) * 0.03f;
        material.SetFloat(brokenProperty, brokenValue);
        material.SetFloat(brokenPointsProperty, brokenPoints);       
    }
}
