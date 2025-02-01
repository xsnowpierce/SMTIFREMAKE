using System;
using System.Linq;
using UnityEngine;

namespace Game.Audio
{
    [CreateAssetMenu(fileName = "New Sound Library", menuName = "Sound/Sound Library")]
    public class SFXLibrary : ScriptableObject
    {
        [Serializable]
        public struct Sound
        {
            public string id;
            public AudioClip[] clips;
            [Range(0, 1f)] public float volume;
        }

        [SerializeField] private string sfxLibraryName;
        [SerializeField] private Sound[] sounds;

        public string GetLibraryName() => sfxLibraryName;

        public Sound GetSoundFromID(string soundID)
        {
            Sound chosen = sounds.FirstOrDefault(sound => sound.id.Equals(soundID));

            if (chosen.clips == null)
            {
                Debug.LogError("Sound was called but there was no ID to match key '" + soundID + "'.");
            }

            else if (chosen.clips.Length == 0)
            {
                Debug.LogWarning("Sound '"+ soundID + "' was called but there weren't enough clips to play.");
            }

            return chosen;
        }

    }
}