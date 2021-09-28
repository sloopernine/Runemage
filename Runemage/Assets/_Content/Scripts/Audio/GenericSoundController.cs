using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


// Call on this to play OneShot Sounds in the world through script
public class GenericSoundController : MonoBehaviour
{
    public AudioMixer audioMixer;
    private static GenericSoundController instance;
    public static GenericSoundController Instance { get => instance; }

    [Tooltip("The max number of sounds (AudioSources) allowed to play. This many Audiosources are created in Awake")]
    [SerializeField] int voicePoolSize = 24;

    private List<AudioSource> audioSources;
    [SerializeField] GameObject audioSourcePrefab;

    private readonly string soundsPath = "WorldSounds";

    private Dictionary<WorldSounds, List<AudioClip>> soundLibrary;


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

        soundLibrary = new Dictionary<WorldSounds, List<AudioClip>>();
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

            bool defined = System.Enum.IsDefined(typeof(WorldSounds), name);

            if (defined)
            {
                WorldSounds sound = (WorldSounds)System.Enum.Parse(typeof(WorldSounds), name);
                if (soundLibrary.ContainsKey(sound) == false || soundLibrary[sound] == null)
                {
                    soundLibrary[sound] = new List<AudioClip>();
                }
                soundLibrary[sound].Add(clips[index]);

            }
            else
            {
                Debug.LogWarning("[SoundSystem] WorldSound: Found clip for sound that is not in enumeration: " + clips[index].name);
            }
        }
    }

    public void Play(WorldSounds sound, Vector3 position, bool randomizePitch = true)
    {
        if (!soundLibrary.ContainsKey(sound))
        {
            Debug.LogError("[SoundSystem] WorldSound: Trying to play sound without a clip. Need a clip at: " + soundsPath + "/" + sound.ToString());
            sound = WorldSounds._default;
            return;
        }

        AudioSource source = GetIdleAudioSource();

        source.clip = soundLibrary[sound][Random.Range(0, soundLibrary[sound].Count)];
        source.transform.position = position;
        AudioMixerGroup[] channels = audioMixer.FindMatchingGroups(sound.ToString());
        if (channels.Length > 0)
        {
            source.outputAudioMixerGroup = channels[0]; 
        }
        else
        {
            source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        }

        if (randomizePitch)
        {
            source.pitch = Random.Range(0.7f, 1.3f);
        }
        else
        {
            source.pitch = 1f;
        }
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
        Debug.LogWarning("[SoundSystem] WorldSound: No idle audioSource found, adding new one. Consider increasing VoicePoolSize");

        return s;

    }
}
