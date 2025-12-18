using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    /*
    //instance of MusicManager
    public static MusicManager instance; 

    //time fields
    public double musicDuration;
    public double goalTime;

    //audio fields
    [SerializeField] private AudioSource[] musicSources;
    [SerializeField] private AudioSource musicObject;
    private AudioClip music;
    int audioToggle = 0;


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

    public void OnPlayMusic(AudioClip musicClip, Transform spawnTransform, float volume)
    {
    
        musicSources[audioToggle] = Instantiate(musicObject, spawnTransform.position, Quaternion.identity);
        musicSources[audioToggle + 1] = Instantiate(musicObject, spawnTransform.position, Quaternion.identity);

        music = musicClip;
        musicSources[audioToggle].clip = music;
        musicSources[audioToggle + 1].clip = music;

        musicDuration = (double)music.samples / music.frequency;
        goalTime = goalTime + musicDuration;

        Debug.Log("goalTime:" + goalTime);

        musicSources[audioToggle].Play();
    }

    private void Update()
    {
        Debug.Log("dsp.Time:" + AudioSettings.dspTime);

        if (AudioSettings.dspTime > goalTime)
        {
            PlayScheduledClip();
        } 
    }

    private void PlayScheduledClip()
    {
        musicSources[audioToggle].PlayScheduled(goalTime);

        musicDuration = (double)music.samples / music.frequency;
        goalTime = goalTime + musicDuration;

        audioToggle = 1 - audioToggle;
    }
    */
}
