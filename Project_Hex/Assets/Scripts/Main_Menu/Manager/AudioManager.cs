using FMODUnity;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] public StudioEventEmitter MusicEmitter;
    [SerializeField] public EventReference BackgroundMusic;
    [SerializeField] public EventReference VictoryMusic;
    [SerializeField] public EventReference DefeatMusic;
    [SerializeField] public EventReference clickSound;

    [SerializeField] public EventReference PlayCardSound;
    [SerializeField] public EventReference DiscardCardSound;
    [SerializeField] public EventReference DrawCardSound;
    [SerializeField] public EventReference SummonPPermanentSound;
    [SerializeField] public EventReference SummonEPermanentSound;
    [SerializeField] public EventReference DieSound;
    [SerializeField] public EventReference HollowDieSound;

    void Start()
    {
        PlayMusic(BackgroundMusic);
    }

    public void PlayMusic(EventReference Music)
    {
        if (MusicEmitter == null)
        {
            MusicEmitter = gameObject.AddComponent<StudioEventEmitter>();
        }

        MusicEmitter.EventReference = Music;
        MusicEmitter.Play(); // Lance la musique (en boucle si ton event FMOD est en loop)
    }

    public void StopMusic()
    {
        if (MusicEmitter != null)
        {
            MusicEmitter.Stop();
        }
    }

    public void ChangeMusic(EventReference NewMusic)
    {
        StopMusic();
        PlayMusic(NewMusic);
    }

    public void PlayClickSound()
    {
        RuntimeManager.PlayOneShot(clickSound);
    }    
    
    public bool IsValid(EventReference eventref)
    {
        return !eventref.IsNull && !string.IsNullOrEmpty(eventref.Path);
    }
}
