using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovingSoundController : MonoBehaviour
{
    private static MovingSoundController instance;
    public static MovingSoundController Instance { get => instance; }

    //public CollisionSoundList sounds;

    [Tooltip("The max number of sounds (AudioSources) allowed to play. This many Audiosources are created in Awake")]
    [SerializeField] int voicePoolSize = 24;

    private List<AudioSource> audioSources;
    [SerializeField] GameObject audioSourcePrefab;

    private readonly string movingSoundsPath = "MovingSounds";

    private Dictionary<MovingSounds, List<AudioClip>> soundLibrary;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSources = new List<AudioSource>();

        for (int i = 0; i < voicePoolSize; i++)
        {
            AudioSource s = Instantiate<GameObject>(audioSourcePrefab).GetComponent<AudioSource>();
            audioSources.Add(s);
            s.transform.parent = this.transform;
        }

        soundLibrary = new Dictionary<MovingSounds, List<AudioClip>>();
        LoadSoundLibrary();

    }

    private void LoadSoundLibrary()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>(movingSoundsPath);
        for (int index = 0; index < clips.Length; index++)
        {
            string name = clips[index].name;
            int dividerIndex = name.IndexOf("__");
            if (dividerIndex >= 0)
                name = name.Substring(0, dividerIndex);

            bool defined = System.Enum.IsDefined(typeof(MovingSounds), name);

            if (defined)
            {
                MovingSounds mat = (MovingSounds)System.Enum.Parse(typeof(MovingSounds), name);
                if (soundLibrary.ContainsKey(mat) == false || soundLibrary[mat] == null)
                {
                    soundLibrary[mat] = new List<AudioClip>();
                }
                soundLibrary[mat].Add(clips[index]);

            }
            else
            {
                Debug.LogWarning("[SoundSystem] CollisionSound: Found clip for material that is not in enumeration: " + clips[index].name);
            }
        }
    }

    public void Play(MovingSounds mat, Transform newParent, float impactVolume = 1f)
    {
        if (!soundLibrary.ContainsKey(mat))
        {
            Debug.LogError("[SoundSystem] MovingSound: Trying to play sound for material without a clip. Need a clip at: " + movingSoundsPath + "/" + mat.ToString());
            mat = MovingSounds._default;
            return;
        }

        AudioSource source = GetIdleAudioSource();
        source.clip = soundLibrary[mat][Random.Range(0, soundLibrary[mat].Count)];
        source.transform.parent = newParent;
        source.pitch = Random.Range(0.95f, 1.05f);
        source.Play();

    }
    private AudioSource GetIdleAudioSource()
    {
        if (audioSources.Exists(source => !source.isPlaying))
        {
            return audioSources.Find(source => !source.isPlaying);
        }
        AudioSource s = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
        audioSources.Add(s);
        Debug.LogWarning("[SoundSystem] CollisionSound: No idle audioSource found, adding new one. Consider increasing VoicePoolSize");

        return s;

    }
}
