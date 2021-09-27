using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationEventHandler : MonoBehaviour
{
    private Enemy enemyInParent;

    private void Start()
    {
        enemyInParent = GetComponentInParent<Enemy>();
    }

    public void AnimationDealDamage()
    {
        enemyInParent.AnimationDealDamage();
    }

}
