using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private AudioSource loopingAudioSource;


    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // <-- persists between scenes
        }
        else
        {
            Destroy(gameObject);           // <-- prevents duplicates
        }
    }

    //plays FX Clip until clip is finished
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        if (audioClip == null) return;

        // Create a temporary audio source
        AudioSource tempAudioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        tempAudioSource.clip = audioClip;
        tempAudioSource.volume = volume;

        tempAudioSource.Play();

        // Destroy after clip finishes
        Destroy(tempAudioSource.gameObject, tempAudioSource.clip.length);
    }


    //plays loop of FX Clip
    public void PlayLoopingFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        if (audioClip == null) return;

        // Stop and destroy any existing looping audio first
        DestroyLoopingFXClip();

        // Create a new looping audio source
        loopingAudioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        loopingAudioSource.clip = audioClip;
        loopingAudioSource.volume = volume;

        loopingAudioSource.loop = true;
        loopingAudioSource.Play();
    }

    //destroys looping Clip
    public void DestroyLoopingFXClip()
    {
        if (loopingAudioSource != null)
        {
            Destroy(loopingAudioSource.gameObject);
            loopingAudioSource = null;
        }
    }
}