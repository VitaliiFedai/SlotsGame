using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [Serializable]
    public struct SaveData
    {
        public float _soundVolume;
        public float _musicVolume;
        public bool _mute;

        public SaveData(SoundManager soundManager)
        {
            _musicVolume = soundManager.GetMusicVolume();
            _soundVolume = soundManager.GetSoundVolume();
            _mute = soundManager.GetMute();
        }

        public void SetSavedDataTo(SoundManager soundManager)
        {
            soundManager.SetMusicVolume(_musicVolume);
            soundManager.SetSoundVolume(_soundVolume);
            soundManager.SetMute(_mute);
        }
    }

    public event Action OnChange;

    [SerializeField] private AudioClip _clickClip;
    [SerializeField] private AudioClip _coinsDropClip;
    [SerializeField] private AudioClip _popUpClip;
    [SerializeField] private AudioListener _listener;

    public bool IsPlaying => _source.isPlaying;
    
    public float GetSoundVolume()
    {
        return _soundVolume;    
    }

    public void SetSoundVolume(float volume)
    {
        _soundVolume = volume;
        OnChange?.Invoke();
    }

    private float _soundVolume = 1f;
    private AudioSource _source;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        SetMusicVolume(_source.playOnAwake ? 1.0f : 0f) ;
    }

    public void SetMusicVolume(float volume)
    {
        _source.volume = volume;
        OnChange?.Invoke();
    }

    public float GetMusicVolume()
    { 
        return _source.volume;    
    }

    public void SetMute(bool value)
    { 
        _source.mute = value;
        OnChange?.Invoke();
    }

    public bool GetMute()
    { 
        return _source.mute;
    }

    public void Play()
    {
        _source.Play();
    }

    public void Stop() 
    {
        _source.Stop();
    }

    internal SaveData GetSaveData()
    {
        return new SaveData(this);
    }

    public void PlayCoinsDropSound()
    {
        PlaySound(_coinsDropClip, _soundVolume);
    }

    public void PlayClickSound()
    {
        PlaySound(_clickClip, _soundVolume);
    }

    public void PlayPopUpSound()
    {
        PlaySound(_popUpClip, _soundVolume);
    }

    private void PlaySound(AudioClip audioClip, float volume)
    {
        if (!GetMute())
        {
            AudioSource.PlayClipAtPoint(audioClip, _listener.transform.position, volume);
        }
    }

}
