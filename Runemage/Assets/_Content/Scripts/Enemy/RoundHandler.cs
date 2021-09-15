using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roundInformationText;
    [SerializeField] Round[] rounds;
    private Round currentRound;
    private int numberOfRounds;
    private int roundIndex;
    private bool isRoundDead;
    private float waveTimer;
    private int roundTotalEnemies;
    public int getRoundTotalEnemies { get { return roundTotalEnemies; } }

    private void Start()
    {
        numberOfRounds = rounds.Length-1;

        if (rounds != null)
        {
            currentRound = rounds[roundIndex];
        }
    }

    //Later StartRound will be called somewhere in the Scenes environment?
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) && isRoundDead)
        {
            roundInformationText.text = "You started a new Round: " + currentRound.name;
            StartRound(currentRound); 
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("All enemies are not dead");
        }
    }

    private void StartRound(Round round)
    {
        isRoundDead = false;
        EnemyWave[] enemyWaves = round.enemyWaves;
        waveTimer = round.setWaveTimer;
        StartCoroutine(SpawnWaves(enemyWaves));
    }

    private IEnumerator SpawnWaves(EnemyWave[] nextWaves)
    {
        foreach (EnemyWave wave in nextWaves)
        {
            roundTotalEnemies += wave.numberOfEnemies * wave.numberOfSpawners;
        }
        foreach (EnemyWave wave in nextWaves)
        {
            yield return StartCoroutine(StartWave(wave));
        }       
    }

    private IEnumerator StartWave(EnemyWave nextWave)
    {
        //Hide text if Debug is inActivated later?
        roundInformationText.text = "New Wave: " + nextWave.name;

        EnemySpawnerHandler spawnerHandler = GetComponent<EnemySpawnerHandler>();

        if (spawnerHandler == null)
        {
            Debug.LogWarning("Round Handler is missing Enemy Spawner Handler. Canceled Start Round()");
            yield return null;
        }

        spawnerHandler.nextWave = nextWave;
        spawnerHandler.startWave = true;
        yield return new WaitForSeconds(waveTimer);
    }


    //Later implement a GlobalSignal that signals it is a new round.
    public void UpdateRound()
    {
        Debug.Log("0.Update Current Round");
        roundIndex++;
        isRoundDead = true;
        roundTotalEnemies = 0;
        if (roundIndex <= numberOfRounds)
        {
            Debug.Log("1.New Round");
            roundInformationText.text = "Press N to start new Wave";
            currentRound = rounds[roundIndex];
        }
        else
        {
            Debug.Log("2.No more rounds. GZ YOU WON THE GAME");
        }
    }
}