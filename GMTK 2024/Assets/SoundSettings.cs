using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Game
{
    public class SoundSettings : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterSlider; 
        [SerializeField] private Slider musicSlider; 
        [SerializeField] private Slider soundSlider;
        void Awake()
        {
            float masterVol = PlayerPrefs.HasKey("MasterVolume") ? PlayerPrefs.GetFloat("MasterVolume") : 1;
            float musicVol = PlayerPrefs.HasKey("MusicVolume") ? PlayerPrefs.GetFloat("MusicVolume") : 1;
            float soundVol = PlayerPrefs.HasKey("SoundFXVolume") ? PlayerPrefs.GetFloat("SoundFXVolume") : 1;
            SetMasterVolume(masterVol);
            SetMusicVolume(musicVol);  
            SetSoundVolume(soundVol);
            LoadSlider(masterVol,musicVol,soundVol);

        }
        public void SetMasterVolume(float level)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(level)*20f);
            PlayerPrefs.SetFloat("MasterVolume", level);
        }

        public void SetMusicVolume(float level)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
            PlayerPrefs.SetFloat("MusicVolume", level);
        }

        public void SetSoundVolume(float level)
        {
            audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(level) * 20f);
            PlayerPrefs.SetFloat("SoundFXVolume", level);

        }

        void LoadSlider(float master, float music, float sound)
        {
            if (masterSlider == null) return;
            masterSlider.value = master;
            if(musicSlider == null) return; 
            musicSlider.value = music;  
            if(soundSlider == null) return; 
            soundSlider.value = sound; 
        }
    }
}
