using System;
using UnityEngine;

namespace Game.Level
{
    public class MusicCollider : MapCollider
    {
        [SerializeField] private string musicID;

        public string GetMapID() => musicID;
    }
}