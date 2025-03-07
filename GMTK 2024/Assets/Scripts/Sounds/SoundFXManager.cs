using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Splines;

namespace Game
{
    public class SoundFXManager : MonoBehaviour
    {
        public static SoundFXManager _instance;
        [field: SerializeField]
        public AudioMixerGroup SoundFXGroup { get; private set; }

        public static SoundFXManager Instance
        {
            get
            {
                _instance = FindObjectOfType<SoundFXManager>();
                //_instance ??=new GameObject("SoundManager").AddComponent<SoundFXManager>();
                return _instance;
            }
        }

        private void Awake()
        {
            if(Instance != this)
            {
                Destroy(this.gameObject);
            }
            DontDestroyOnLoad(this.gameObject);
        }

        public void PlaySoundClip(AudioClip clip, Transform spawnTransform, float volume)
        {
            AudioSource audioSource = new GameObject("OneShotAudio").AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = SoundFXGroup;
            audioSource.spatialBlend = 1;
            audioSource.transform.position = spawnTransform.position;
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();
            Destroy(audioSource.gameObject, audioSource.clip.length);
        }

        public void PlayRandomSoundClip(AudioClip[] clips, Transform spawnTransform, float volume)
        {

            if (clips == null) return;
            if(clips.Length == 0) return;
            AudioClip selectedClip = clips[Random.Range(0,clips.Length)];
            AudioSource audioSource = new GameObject("OneShotAudio").AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = SoundFXGroup;
            audioSource.spatialBlend = 1;
            audioSource.transform.position = spawnTransform.position;
            audioSource.clip = selectedClip;
            audioSource.volume = volume;
            audioSource.Play();
            Destroy(audioSource.gameObject, audioSource.clip.length);
        }
    }
}
