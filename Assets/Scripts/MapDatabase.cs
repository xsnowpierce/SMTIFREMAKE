using System;
using System.Linq;
using Game.Level;
using UnityEngine;

namespace Database
{
    [CreateAssetMenu(fileName = "New Map Database", menuName = "Database/Maps")]
    public class MapDatabase : ScriptableObject
    {
        [SerializeField] private GameObject[] maps;

        public GameObject[] GetMaps() => maps;
    }
}