using System;
using TMPro;
using UnityEngine;

public class SpellCastOrigin : PC_Interactable
{
    public GameObject fireBallPrefab;
    public GameObject icePrefab;

    public Spell currentSpell;

    private MeshRenderer meshRenderer;
    private TextMeshPro tmp;

    [SerializeField] bool destroyOnCast;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        tmp = GetComponentInChildren<TextMeshPro>();
        switch (currentSpell)
        {
            case Spell.Fireball:
                SetVisuals(Color.red);
                tmp.text = "Fire";
                break;
            case Spell.Ice:
                SetVisuals(Color.cyan);
                tmp.text = "Ice";
                break;
            default:
                break;
        }

    }

    private void SetVisuals(Color c)
    {
        c.a = 0.5f;
        meshRenderer.material.color = c;
        //tmp.rectTransform.rotation = transform.rotation;
    }

    public void CastSpell(Vector3 position)
    {
        switch (currentSpell)
        {
            case Spell.Fireball:
                Fireball(position);
                break;
            case Spell.Ice:
                createIceSpear(position);
                break;
            default:
                break;
        }

        if (destroyOnCast)
        {
            Destroy(gameObject);
        }
    }

    private void Fireball(Vector3 position)
    {
        GameObject obj = Instantiate(fireBallPrefab);
        obj.transform.position = transform.position;

    }

    private void createIceSpear(Vector3 position)
    {
        GameObject obj = Instantiate(icePrefab);
        obj.transform.position = transform.position;
        
    }

    public override void Release(Vector3 force)
    {
        transform.parent = null;
    }

    public override void Grab(Transform parent)
    {
        transform.SetParent(parent);
    }
}
