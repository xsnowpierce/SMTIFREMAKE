using System.Linq;
using Battle;
using Game.Level;
using Music;
using UnityEngine;

namespace Game.Battle
{
    [RequireComponent(typeof(AudioSource))]
    public class BattleMusic : MonoBehaviour
    {
        [SerializeField] private string encounterSongID;
        [SerializeField] private string battleSongID;
        [SerializeField] private string bossBattleSongID;
        [SerializeField] private string victorySongID;
        [SerializeField] private string levelupSongID;

        public void PlayMusic(BattlePhase phase, BattleType battleType)
        {
            string songID;
            bool instantStart = false;
            switch(phase)
            {
                case BattlePhase.Talking:
                    songID = encounterSongID;
                    break;
                case BattlePhase.Attacking:
                    instantStart = true;
                    switch (battleType)
                    {
                        case BattleType.RandomEncounter:
                            songID = battleSongID;
                            break;
                        case BattleType.Boss:
                            songID = bossBattleSongID;
                            break;
                        default:
                            songID = battleSongID;
                            break;
                    }
                    //else if(battleType == BattleType.Fiend)
                    break;
                case BattlePhase.Finished:
                    songID = victorySongID;
                    break;
                case BattlePhase.LevelUp:
                    songID = levelupSongID;
                    break;
                default:
                    instantStart = true;
                    songID = encounterSongID;
                    break;
            }
            
            MusicController musicController = FindObjectOfType<MusicController>();
            MusicPack.MusicInfo info = musicController.GetCurrentMusicPack().music.FirstOrDefault(info => info.musicID.Equals(songID));
            if(instantStart) musicController.InstantStartSong(info);
            else musicController.ChangeToSong(info);
        }
    }
}