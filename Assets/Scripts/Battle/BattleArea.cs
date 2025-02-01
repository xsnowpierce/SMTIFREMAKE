using System;
using Battle;
using Entity;
using Game.Encounter;
using Game.Movement;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.Collision
{
    public class BattleArea : MonoBehaviour
    {
        [Serializable]
        public struct EnemyEncounter
        {
            [Range(0f, 1f)] public float encounterRate;
            public BattleController.Enemy[] demons;
        }
        
        private EnemyEncounterController encounter;
        [SerializeField] private EnemyEncounter[] encounters;

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                encounter = other.GetComponentInParent<EnemyEncounterController>();
                if (encounter != null)
                {
                    // Is the player
                    encounter.AddBattleArea(this);
                }
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                encounter = other.GetComponentInParent<EnemyEncounterController>();
                if (encounter != null)
                {
                    // Is the player
                    encounter.RemoveBattleArea(this);
                }
            }
        }

        public EnemyEncounter[] GetEnemies => encounters;
    }
}