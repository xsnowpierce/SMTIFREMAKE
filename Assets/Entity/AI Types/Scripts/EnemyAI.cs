using Entity.Skills;
using Game.Battle;
using UnityEngine;

namespace AI
{
    public abstract class EnemyAI : ScriptableObject
    {
        [Range(0f, 1f)] public float meleePercentage; 
        public abstract Skill DecideAttack(Entity.Demon currentEntity, int currentHealth, bool isBackRow);
        public abstract int[] DecideAttacking(Entity.Entity[] partyMembers, BattleEnemySelection.SelectionType targetType, float backrowPercent, bool isMelee);
    }
}