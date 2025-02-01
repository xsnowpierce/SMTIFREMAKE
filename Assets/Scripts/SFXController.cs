using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SFXController : MonoBehaviour
    {

        private AudioSource source;
        [SerializeField] private SFXLibrary sfxLibrary;
        [SerializeField] [Range(0f, 1f)] private float volume;
        
        private void Awake()
        {
            source = GetComponent<AudioSource>();
            source.volume = volume;
        }

        public void PlaySound(string soundID)
        {
            SFXLibrary.Sound chosen = sfxLibrary.GetSoundFromID(soundID);
            
            int random = Random.Range(0, chosen.clips.Length);
            AudioClip soundToPlay = chosen.clips[random];
            float volume = Mathf.Clamp(chosen.volume, 0f, 1f);
            source.PlayOneShot(soundToPlay, volume);
        }

        public float GetVolume() => volume;

        public bool isPlaying() => source.isPlaying;
    }
}