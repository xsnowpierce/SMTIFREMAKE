using System;
using AI;
using Entity.Skills;
using Game.Battle;
using Items;
using UnityEngine;

namespace Entity
{
    [CreateAssetMenu(fileName = "New Demon", menuName = "Entity/Demon")]
    public class Demon : Entity
    {
        [Header("Demon Settings")]
        public EnemyAI aiType;
        public BattleEnemySelection.SelectionType meleeAttackType = BattleEnemySelection.SelectionType.Single;
        public int costPerTurn;
        public Element inheritType;
        [Space(10)] 
        public Item[] droppedItems;
        
        [Header("Alignment Settings")] 
        [Range(0f, 1f)] public float darkLight;
        [Range(0f, 1f)] public float chaosLaw;

        public override int GetAttackPower()
        {
            return (level + stats.strength) * 2;
        }

        public override int GetAttackAccuracy()
        {
            return Convert.ToInt32(level * 1.5 + stats.agility + (stats.strength + stats.luck) / 4f);
        }

        public override int GetDefense()
        {
            return (level + stats.strength) * 2;
        }

        public override int GetEvasion()
        {
            return Convert.ToInt32(level * 1.5 + stats.agility + (stats.intelligence + stats.luck) / 4f);
        }
    }
}