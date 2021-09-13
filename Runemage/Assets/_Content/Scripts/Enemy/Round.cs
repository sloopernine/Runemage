using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Round", menuName = "ScriptableObjects/Round", order = 2)]
public class Round : ScriptableObject
{
    public EnemyWave[] enemyWaves;
    public float setWaveTimer;
}
