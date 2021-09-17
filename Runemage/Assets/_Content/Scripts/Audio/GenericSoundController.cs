using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorldSounds
{
    IceSpearCreate,
    IceSpearExplode,
}

// Call on this to play OneShot Sounds in the world through script
public class GenericSoundController : MonoBehaviour
{
    private static GenericSoundController instance;
    public static GenericSoundController Instance { get => instance; }

    [Tooltip("The max number of sounds (AudioSources) allowed to play. This many Audiosources are created in Awake")]
    [SerializeField] int voicePoolSize = 24;

    private List<AudioSource> audioSources;
    [SerializeField] GameObject audioSourcePrefab;

    private readonly string soundsPath = "CollisionSounds";

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
            AudioSource s = Instantiate(audioSourcePrefab).GetComponent<AudioSource>();
            audioSources.Add(s);
            s.transform.parent = this.transform;
        }

        soundLibrary = new Dictionary<CollisionSoundMaterial, List<AudioClip>>();
        LoadSoundLibrary();

    }

    private void LoadSoundLibrary()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>(soundsPath);
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
            Debug.LogError("[SoundSystem] CollisionSound: Trying to play sound for material without a clip. Need a clip at: " + soundsPath + "/" + mat.ToString());
            mat = CollisionSoundMaterial._default;
            return;
        }

        AudioSource source = GetIdleAudioSource();
        source.clip = soundLibrary[mat][Random.Range(0, soundLibrary[mat].Count)];
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
}
