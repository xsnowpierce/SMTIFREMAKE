using System.Collections.Generic;
using UnityEngine;

namespace Database
{
    [CreateAssetMenu(fileName = "New Sprite Database", menuName = "Database/Sprite")]
    public class SpriteDatabase : ScriptableObject
    {
        public List<Sprite> spriteDatabase;

        public List<Sprite> GetSpriteDatabase() => spriteDatabase;

        public Sprite GetSprite(int index)
        {
            return spriteDatabase[index];
        }
    }
}