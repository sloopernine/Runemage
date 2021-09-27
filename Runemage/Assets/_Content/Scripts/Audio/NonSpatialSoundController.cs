using _Content.Scripts.Data.Containers.GlobalSignal;
using Data.Enums;
using Data.Interfaces;
using Singletons;
using System;
using System.Collections;
using UnityEngine;


public class NonSpatialSoundController : MonoBehaviour, IReceiveGlobalSignal
{
    [SerializeField] AudioClip pausedMusicClip;
    [SerializeField] AudioClip ambienceClip;
    //[SerializeField] AudioClip PlayMusic;
    [SerializeField] GameObject audioSourcePrefab;


    private AudioSource pauseMusic;
    private AudioSource battleMusic;
    private AudioSource ambience;

    //used for division so needs to be non-zero
    [Range(0.01f, 20f)]
    [SerializeField] float audioFadeTime;

    private void Start()
    {
        pauseMusic = InitializeAudioSource(pausedMusicClip);
        ambience = InitializeAudioSource(ambienceClip);

        

    }

    private void OnEnable()
    {
        GlobalMediator.Instance.Subscribe(this);
    }

    private void OnDisable()
    {
        GlobalMediator.Instance.UnSubscribe(this);

    }
    private AudioSource InitializeAudioSource(AudioClip clip)
    {
        AudioSource audioSource = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
        audioSource.transform.SetParent(transform);
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = 0;
        return audioSource;
    }


    public void ReceiveGlobal(GlobalEvent eventState, GlobalSignalBaseData globalSignalData = null)
    {
        switch (eventState)
        {
            case GlobalEvent.WIN_GAMESTATE:
                break;
            case GlobalEvent.LOST_GAMESTATE:
                break;
            case GlobalEvent.PAUSED_GAMESTATE:
                OnGameStatePaused();
                break;
            case GlobalEvent.PLAY_GAMESTATE:
                OnGameStatePlay();
                break;
            default:
                break;
        }
    }

    private void OnGameStatePaused()
    {
        StopAllCoroutines();
        StartCoroutine(AudioFadeOut(ambience, audioFadeTime));
        StartCoroutine(AudioFadeIn(pauseMusic, audioFadeTime));

    }

    private void OnGameStatePlay()
    {
        StopAllCoroutines();
        StartCoroutine(AudioFadeOut(pauseMusic, audioFadeTime));
        StartCoroutine(AudioFadeIn(ambience, audioFadeTime));

    }

    private IEnumerator AudioFadeIn(AudioSource source, float fadeInTime)
    {

        print($"Fading In {source.clip}");
        if (!source.isPlaying)
        {
            source.Play();
        }

        for (float f = source.volume; f < 1f; f += Time.deltaTime / fadeInTime)
        {
            source.volume = f;
            yield return null;
        }

        print("finished fade");

    }

    private IEnumerator AudioFadeOut(AudioSource source, float fadeOutTime)
    {
        print($"Fading out {source.clip}");


        for (float f = source.volume; f > 0; f -= Time.deltaTime / fadeOutTime)
        {
            source.volume = f;
            yield return null;
        }

        source.Stop();
        print("finished fade");

    }

}
