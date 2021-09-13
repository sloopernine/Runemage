using System;
using TMPro;
using UnityEngine;

public class SpellCastOrigin : MonoBehaviour
{

    public GameObject fireBallPrefab;
    public GameObject RockPrefab;

    public Spell currentSpell;

    private MeshRenderer meshRenderer;
    private TextMeshPro tmp;

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
            case Spell.Teleport:
                SetVisuals(Color.magenta);
                tmp.text = "Teleport";

                break;
            case Spell.CreateRock:
                SetVisuals(Color.gray);
                tmp.text = "Rock";

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
            case Spell.Teleport:
                Teleport(position);
                break;
            case Spell.CreateRock:
                CreateRock(position);
                break;
            default:
                break;
        }
    }

    private void Fireball(Vector3 position)
    {
        GameObject obj = Instantiate(fireBallPrefab);
        obj.transform.position = transform.position;
        obj.transform.LookAt(transform.position * 2 - position);

    }

    private void Teleport(Vector3 position)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000f))
        {
            Debug.DrawLine(transform.position, transform.forward * 1000f, Color.red, 3f);

            Vector3 newPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.position = newPosition;

        }
        
    }

    private void CreateRock(Vector3 position)
    {
        GameObject obj = Instantiate(RockPrefab);
        obj.transform.position = transform.position + transform.position - position + transform.up;
        
    }
}
