using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;

    [SerializeField] private AudioSource soundFXObject;
    
    public AudioSource audioSource;


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
        audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;

        audioSource.Play();

        // Destroy after clip finishes
        Destroy(audioSource.gameObject, audioSource.clip.length);
    }

    
    //plays loop of FX Clip
    public void PlayLoopingFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        if (audioClip == null) return;

        // Create a temporary audio source
        audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;

        audioSource.loop = true;
        audioSource.Play();

    }

    //destroys looping Clip
    public void DestroyLoopingFXClip()
    {
        Destroy(audioSource.gameObject);
    }

    //pause audio when game is paused
    public void PauseClip()
    {
        audioSource.Pause();
    }

    //resume audio when game is unpaused
    public void ResumeClip()
    {
        audioSource.UnPause();
    }
}
