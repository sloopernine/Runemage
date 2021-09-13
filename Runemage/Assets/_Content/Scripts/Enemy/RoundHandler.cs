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
    public bool isRoundDead;
    private float waveTimer;

    [Header("Testing")]
    public bool updateRound;

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

        if (updateRound)
        {
            UpdateCurrentRound();
        }
    }

    private void StartRound(Round round)
    {
        EnemyWave[] enemyWaves = round.enemyWaves;
        waveTimer = round.setWaveTimer;
        StartCoroutine(SpawnWaves(enemyWaves));
    }

    private IEnumerator SpawnWaves(EnemyWave[] nextWaves)
    {
        foreach (EnemyWave waves in nextWaves)
        {
            yield return StartCoroutine(StartWave(waves));
        }
    }

    private IEnumerator StartWave(EnemyWave nextWave)
    {
        //Hide text if Debug is inActivated later?
        roundInformationText.text = "New Wave: " + nextWave.name;

        EnemySpawnerHandler spawnerHandler = GetComponent<EnemySpawnerHandler>();

        if (spawnerHandler == null)
        {
            Debug.LogWarning("Round Handler is missing Enemy Spawner Handler. Canceld 'Start Round()'");
            yield return null;
        }

        spawnerHandler.nextWave = nextWave;
        spawnerHandler.startWave = true;
        yield return new WaitForSeconds(waveTimer);
    }


    //I want to acivate this when we know that the current waves are cleared.
    public void UpdateCurrentRound()
    {
        Debug.Log("Update Current Round");
        roundIndex++;
        if (roundIndex <= numberOfRounds)
        {
            currentRound = rounds[roundIndex];
        }
        else
        {
            Debug.Log("No more rounds. GZ YOU WON THE GAME");
        }
        updateRound = false;
    }
}