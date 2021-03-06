using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data.Interfaces;
using Data.Enums;
using _Content.Scripts.Data.Containers.GlobalSignal;
using Singletons;

public class RunestoneMovement : MonoBehaviour, IReceiveGlobalSignal
{
    private Vector3 startPosition;
    [SerializeField] Vector3 movePosition;
    [SerializeField] float speed = 1;
    [SerializeField] List<ParticleSystem> particels;
    [Tooltip ("If false runestone moves under ground during play")]
    [SerializeField] bool alwaysShow; 
    private Rigidbody rigidBody;
    private bool setPosition;
    private bool isMoving;
    [SerializeField] bool showOnWictory;
    private bool IsMoving { get { return isMoving; } set { if (value == isMoving) return; isMoving = value; } }


    private void Start()
    {
        startPosition = transform.position;
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if(setPosition)
        {
            MoveTowardsPoint(movePosition);
        }
        else
        {
            MoveTowardsPoint(startPosition);
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

    private void MoveTowardsPoint(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        float distance = direction.magnitude;

        direction = direction.normalized;

        if (distance > 0.01f)
        {
            isMoving = true;
            rigidBody.MovePosition(transform.position + direction * Time.deltaTime * speed);
        }
        else
        {
            isMoving = false;
        }
        ParticelEffect();
    }

    private void ParticelEffect()
    {
        if (IsMoving)
        {
            foreach (ParticleSystem particle in particels)
            {
                particle.Play();
            }
        }
    }

    public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {
        switch (eventState)
        {
            case GlobalEvent.PAUSED_GAMESTATE:
                if (!alwaysShow && !showOnWictory)
                {
                    setPosition = false;
                    GenericSoundController.Instance.Play(WorldSounds.StoneMoving, transform.position);
                }
                break;

            case GlobalEvent.PLAY_GAMESTATE:
                if (!alwaysShow && !showOnWictory)
                {
                    setPosition = true;
                    GenericSoundController.Instance.Play(WorldSounds.StoneMoving, transform.position);
                }
                break;

            case GlobalEvent.WIN_GAMESTATE:
                if (!alwaysShow && showOnWictory)
                {
                    setPosition = true;
                    GenericSoundController.Instance.Play(WorldSounds.StoneMoving, transform.position);
                }
 
                break;
        }
    }
}
