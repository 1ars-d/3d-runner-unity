using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource _musicSource;
    private AudioSource _effectSource;

    private void Awake()
    {
        _effectSource = GameObject.FindGameObjectWithTag("EffectsSource").GetComponent<AudioSource>();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        _effectSource = GameObject.FindGameObjectWithTag("EffectsSource").GetComponent<AudioSource>();
    }


    public void  ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }

    public void SetMusicVolume(float newVolume)
    {
        _musicSource.volume = newVolume;
    }

    public void PlaySound(AudioClip clip)
    {
        _effectSource.PlayOneShot(clip);
    }
}
