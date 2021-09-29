using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Data.Interfaces;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Singletons;

public class RoundHandler : MonoBehaviour, IReceiveGlobalSignal, ISendGlobalSignal
{
    [SerializeField] TextMeshProUGUI roundInformationText;
    [SerializeField] Round[] rounds;
    private Round currentRound;
    private int numberOfRounds;
    private int roundIndex;
    private bool isRoundDead = true;
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

    private void OnEnable()
    {
        GlobalMediator.Instance.Subscribe(this);
    }

    private void OnDisable()
    {
        GlobalMediator.Instance.UnSubscribe(this);

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
        SendGlobal(GlobalEvent.PLAY_GAMESTATE);
        isRoundDead = false;
        EnemyWave[] enemyWaves = round.enemyWaves;
        waveTimer = round.setWaveTimer;
        StartCoroutine(SpawnWaves(enemyWaves));
    }

    public void StartRoundButton()
    {
        StartRound(currentRound);
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
        SendGlobal(GlobalEvent.PAUSED_GAMESTATE);
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
            SendGlobal(GlobalEvent.WIN_GAMESTATE);
            Debug.Log("2.No more rounds. GZ YOU WON THE GAME");
        }
    }

    //Used by debugger to restart the rounds
    public void SetRound(int newRoundNumber)
    {

        Debug.Log($"Update Current Round to {newRoundNumber}");
        roundIndex = newRoundNumber;
        isRoundDead = true;
        roundTotalEnemies = 0;
       
        roundInformationText.text = "Press N to start new Wave";
        currentRound = rounds[roundIndex];


    }

    public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {
        switch (eventState)
        {
            case GlobalEvent.SET_NEXT_ROUND:
                StopAllCoroutines();
                UpdateRound();
                break;
            case GlobalEvent.SET_ROUND:
                StopAllCoroutines();
                if (globalSignalData is BasicData)
                {
                    BasicData data = (BasicData)globalSignalData;                    
                    SetRound(data.intValue);

                }
                break;

        }
    }

    public void SendGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {
        GlobalMediator.Instance.ReceiveGlobal(eventState, globalSignalData);
    }
}