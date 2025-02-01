using System.Collections;
using Music;
using UnityEngine;

namespace Game.Level
{
    [RequireComponent( typeof(AudioSource))]
    public class MusicController : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;
        [SerializeField] [Range(0f, 1f)] private float targetVolume = 1f;
        [SerializeField] private float fadeMultiplier = 1f;
        [SerializeField] private float fadeWait = 0.5f;
        private Coroutine currentSongLoopWait;
        private string currentMusicID;
        [SerializeField] private MusicPack currentMusicPack;

        public void ChangeToSong(MusicPack.MusicInfo info)
        {
            if (info.song == null) return;
            if (info.musicID.Equals(currentMusicID)) return;

            currentMusicID = info.musicID;
            _source.loop = false;
            
            if (!_source.isPlaying)
            {
                // Play song without fading out last
                if (currentSongLoopWait != null) StopCoroutine(currentSongLoopWait);
                currentSongLoopWait = StartCoroutine(LoopSong(info));
            }
            else
            {
                // Fade out last/current song first
                StartCoroutine(CrossFade(info));
            }
        }

        public void InstantStartSong(MusicPack.MusicInfo info)
        {
            currentMusicID = info.musicID;
            if (currentSongLoopWait != null) StopCoroutine(currentSongLoopWait);
            currentSongLoopWait = StartCoroutine(LoopSong(info));
        }

        public MusicPack GetCurrentMusicPack() => currentMusicPack;
        
        private IEnumerator CrossFade(MusicPack.MusicInfo info)
        {
            for (float i = _source.volume; i > 0; i-= (Time.deltaTime * fadeMultiplier))
            {
                _source.volume = i;
                yield return null;
            }
            
            if(currentSongLoopWait != null) 
                StopCoroutine(currentSongLoopWait);
            
            _source.Stop();
            yield return new WaitForSeconds(fadeWait);
            currentSongLoopWait = StartCoroutine(LoopSong(info));
        }
        
        private IEnumerator LoopSong(MusicPack.MusicInfo info)
        {
            currentMusicID = info.musicID;
            _source.volume = targetVolume;
            _source.clip = info.song;
            _source.Play();

            if (info.songLoop == null)
            {
                _source.loop = true;
                currentSongLoopWait = null;
                yield break;
            }
            else
            {
                _source.loop = false;
            }

            while (_source.isPlaying)
            {
                yield return null;
            }

            _source.clip = info.songLoop;
            _source.Play();
            _source.loop = true;
            currentSongLoopWait = null;
        }

        public void EndSong()
        {
            if(currentSongLoopWait != null) 
                StopCoroutine(currentSongLoopWait);
            StartCoroutine(FadeOutCurrentSong());
        }

        public void StopSong()
        {
            _source.Stop();
            _source.clip = null;
            currentMusicID = "";
            _source.volume = targetVolume;
        }

        private IEnumerator FadeOutCurrentSong()
        {
            for (float i = _source.volume; i > 0; i-= (Time.deltaTime * fadeMultiplier))
            {
                _source.volume = i;
                yield return null;
            }

            _source.volume = 0f;
            _source.Stop();
            _source.clip = null;
            currentMusicID = "";
            _source.volume = targetVolume;
        }
    }
}