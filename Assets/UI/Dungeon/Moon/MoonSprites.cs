using System;
using UnityEngine;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "New Moon Sprites", menuName = "Sprites/Moon")]
    public class MoonSprites : ScriptableObject
    {
        [Serializable]
        public struct Moons
        {
            public Sprite[] sprites;
        }
        
        [SerializeField] private Moons[] moonSprites;

        public Sprite[] GetMoonSprites(int moonNumber) => moonSprites[moonNumber].sprites;
    }
}