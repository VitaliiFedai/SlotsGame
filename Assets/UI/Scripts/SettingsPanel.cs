using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsPanel : PanelUI
{
    private const string BACK_BUTTON_NAME = "back-button";
    private const string MUSIC_VOLUME_SLIDER = "music-volume-slider";
    private const string SOUND_VOLUME_SLIDER = "sound-volume-slider";
    private const string MUTE_TOGGLE = "mute-toggle";

    [SerializeField] private SoundManager _soundManager;

    public event Action OnBackButtonClicked;

    private Toggle _toggle;

    protected override void OnBind()
    {
        SetSliderValues(MUSIC_VOLUME_SLIDER, _soundManager.GetMusicVolume());
        SetSliderValues(SOUND_VOLUME_SLIDER, _soundManager.GetSoundVolume());

        _toggle = GetElement<Toggle>(MUTE_TOGGLE);
        _toggle.value = _soundManager.GetMute();
        
        RegisterCallbacks();
    }

    protected override void OnUnbind()
    {
    }

    private void RegisterCallbacks()
    {
        RegisterOnClickCallback(BACK_BUTTON_NAME, OnBackButtonClicked);
        RegisterOnClickCallback(MUTE_TOGGLE, OnMuteToggleClicked);
        RegisterOnClickCallback(MUSIC_VOLUME_SLIDER, OnSliderClicked);
        RegisterOnClickCallback(SOUND_VOLUME_SLIDER, OnSliderClicked);

        RegisterOnChangeCallback(MUSIC_VOLUME_SLIDER, OnMusicSliderValueChanged);
        RegisterOnChangeCallback(SOUND_VOLUME_SLIDER, OnSoundSliderValueChanged);
    }

    private void OnMusicSliderValueChanged(ChangeEvent<float> evt)
    {
        _soundManager.SetMusicVolume(evt.newValue);

        if (evt.newValue == 0f && _soundManager.IsPlaying)
        {
            _soundManager.Stop();
        }
        else if (evt.newValue > 0f && !_soundManager.IsPlaying)
        {
            _soundManager.Play();
        }
    }

    private void OnSoundSliderValueChanged(ChangeEvent<float> evt)
    {
        _soundManager.SetSoundVolume(evt.newValue);
    }

    private void OnSliderClicked()
    {
        //Do nothing. Just to make UIPanel call OnClick event to play Click sound.
    }

    private void OnMuteToggleClicked()
    {
        _soundManager.SetMute(_toggle.value);
    }

    private void SetSliderValues(string sliderName, float value)
    {
        Slider slider = GetElement<Slider>(sliderName);
        slider.value = value;
        slider.lowValue = 0f;
        slider.highValue = 1f;
    }
}
