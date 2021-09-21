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
    private float reducedDamage;
    private MeshRenderer meshRenderer;
    private bool isBroken;
    public bool getIsBroken { get { return isBroken; } }

    [SerializeField] bool isInvulnerable;

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
        if (isInvulnerable)
        {
            return;
        }

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
    }
}
