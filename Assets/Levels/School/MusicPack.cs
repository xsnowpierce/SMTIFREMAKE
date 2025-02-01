using System;
using System.Linq;
using UnityEngine;

namespace Music
{
    [CreateAssetMenu(fileName = "New Music Pack", menuName = "Sound/Music Pack")]
    public class MusicPack : ScriptableObject
    {
        [Serializable]
        public struct MusicInfo
        {
            public string musicID;
            public AudioClip song;
            public AudioClip songLoop;
        }

        public MusicInfo[] music;
        
        public MusicInfo GetMusicFromString(string id)
        {
            return music.FirstOrDefault(music => music.musicID.Equals(id));
        }
    }
}