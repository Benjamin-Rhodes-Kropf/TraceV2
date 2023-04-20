using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public enum SoundType
    {
        Click, Background, Notification
    }

    [SerializeField] private AudioClip click, background, notification;
    [SerializeField] private AudioSource backgroundAS;
    [SerializeField] private AudioSource audioSource;




    public bool isMusicOn { get; set; }


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
    }


    public void PlayBackGroundSound(bool isOn)
    {
        if (isOn == true)
        {
            backgroundAS.Play();
        }
        else
        {
            backgroundAS.Stop();
        }
    }


    public void PlaySound(SoundType soundType)
    {
        if (true) // Check If Sound is On
        {
            AudioClip audioClip = GetAudioClip(soundType);
            if (this.audioSource.isPlaying)
            {
                AudioSource audioSource = GetAudioSource();
                audioSource.clip = audioClip;
                audioSource.gameObject.AddComponent<DestroyAudioSource>();
                audioSource.Play();
            }
            else
            {
                this.audioSource.Stop();
                this.audioSource.clip = audioClip;
                this.audioSource.Play();
            }
        }
    }



    AudioSource GetAudioSource()
    {
        GameObject audioSourceGameObject = new GameObject();
        AudioSource audioSource = audioSourceGameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        return audioSource;
    }


    private AudioClip GetAudioClip(SoundType soundType)
    {
        return soundType switch
        {
            SoundType.Click => click,
            SoundType.Background => background,
            SoundType.Notification => notification,
            _ => null
        };
    }



}