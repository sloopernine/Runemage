using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSoundController : MonoBehaviour
{
    private static CollisionSoundController instance;
    public static CollisionSoundController Instance { get => instance;}

    //public CollisionSoundList sounds;

    [Tooltip("The max number of sounds (AudioSources) allowed to play. This many Audiosources are created in Awake")]
    [SerializeField] int voicePoolSize = 24;

    [Tooltip("Collision sounds that will produce an impact with a volume lower than this number won't play")]
    [SerializeField] float minCollisionVolume = 0.1f;
    public float MinCollisionVolume { get => minCollisionVolume; set => minCollisionVolume = value; }
    
    [SerializeField] float maxCollisionVelocity = 5;
    public float MaxCollisionVelocity { get => maxCollisionVelocity; set => maxCollisionVelocity = value; }

    private List<AudioSource> audioSources;
    [SerializeField] GameObject audioSourcePrefab;

    private readonly string collisionSoundsPath = "CollisionSounds";

    private Dictionary<CollisionSoundMaterial, List<AudioClip>> soundLibrary;


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

        soundLibrary = new Dictionary<CollisionSoundMaterial, List<AudioClip>>();
        LoadSoundLibrary();

    }

    private void LoadSoundLibrary()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>(collisionSoundsPath);
        for (int index = 0; index < clips.Length; index++)
        {
            string name = clips[index].name;
            int dividerIndex = name.IndexOf("__");
            if (dividerIndex >= 0)
                name = name.Substring(0, dividerIndex);

            bool defined = System.Enum.IsDefined(typeof(CollisionSoundMaterial), name);

            if (defined)
            {
                CollisionSoundMaterial mat = (CollisionSoundMaterial)System.Enum.Parse(typeof(CollisionSoundMaterial), name);
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

    public void Play(CollisionSoundMaterial mat, Vector3 position, float impactVolume = 1f)
    {
        if (!soundLibrary.ContainsKey(mat))
        {
            Debug.LogError("[SoundSystem] CollisionSound: Trying to play sound for material without a clip. Need a clip at: " + collisionSoundsPath + "/" + mat.ToString());
            mat = CollisionSoundMaterial._default;
            return;
        }

        AudioSource source = GetIdleAudioSource();
        source.clip = soundLibrary[mat][Random.Range(0, soundLibrary[mat].Count)];
        source.volume = impactVolume;
        source.transform.position = position;
        source.pitch = Random.Range(0.7f, 1.3f);
        source.Play();

    }
    private AudioSource GetIdleAudioSource()
    {
        if (audioSources.Exists(source => !source.isPlaying))
        {
            return audioSources.Find(source => !source.isPlaying);
        }
        AudioSource s = Instantiate<GameObject>(audioSourcePrefab).GetComponent<AudioSource>();
        audioSources.Add(s);
        Debug.LogWarning("[SoundSystem] CollisionSound: No idle audioSource found, adding new one. Consider increasing VoicePoolSize");

        return s;

    }


    //old system
    //private SoundCue_Collision GetCue(CollisionSoundMaterial mat)
    //{
    //    SoundCue_Collision cue = sounds.soundList.Find(x => x.material == mat);
    //    if (cue == null)
    //    {
    //        Debug.LogWarning($"No Cue set for {mat}");
    //        return null;
    //    }
    //    return cue;
    //}

}
