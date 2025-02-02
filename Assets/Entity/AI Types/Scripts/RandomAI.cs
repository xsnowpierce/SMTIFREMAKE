using System;
using System.Collections.Generic;
using Battle;
using Entity.Skills;
using Entity.Skills.Spells;
using Game.Battle;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Demon
{
    [CreateAssetMenu(fileName = "RandomAI" , menuName = "AI/RandomAI")]
    public class RandomAI : EnemyAI
    {

        public override Skill DecideAttack(Entity.Demon currentEntity, int currentHealth, bool isBackRow)
        {
            // If we are in the back row, we are unable to do a melee attack
            if (!isBackRow)
            {
                float usesSkill = Random.Range(0f, 1f);
                if (usesSkill < meleePercentage)
                {
                    // Melee attack
                    return null;
                } 
            }
            
            // First of all, Let's check to see if we are low in health, and if so, let's then try to do a healing spell!
            if ((float) currentHealth / currentEntity.GetMaxHP() < 0.10f)
            {
                // Try to heal! Do we have any spells?
                HealingSpell skill = null;
                foreach (Skill skills in currentEntity.skills)
                {
                    if (skills is HealingSpell healingSkill)
                    {
                        skill = healingSkill;
                    }
                }

                if (skill == null)
                {
                    // User doesn't have any healing skills, choose a random skill instead
                    int random = Random.Range(0, currentEntity.skills.Length);
                    return currentEntity.skills[random];
                }
                else
                {
                    return skill;
                }
            }
            
            // Health was not less than 10 percent, pick a different skill at random
            List<Skill> usableSkills = new List<Skill>();
            
            // First, check if a healing skill is the only skill we have. Use that if it is
            foreach (Skill skills in currentEntity.skills)
            {
                if (skills is not HealingSpell)
                {
                    usableSkills.Add(skills);
                }
            }

            if (usableSkills.Count == 0)
            {
                // Demon has no usable skills, attack instead
                return null;
            }
            int randomSkill = Random.Range(0, usableSkills.Count);
            return usableSkills[randomSkill];
        }

        public override int[] DecideAttacking(Entity.Entity[] partyMembers, BattleEnemySelection.SelectionType selectionType, float backrowPercent, bool isMelee)
        {
            // If it's a melee attack, we can only target the front row
            
            // Slots 0-2 are front row, 3-5 are back row
            List<int> frontRow = new List<int> { 0, 1, 2 };
            List<int> backRow = new List<int> { 3, 4, 5 };
            
            // Check if the users are alive/active
            for (int i = 0; i < partyMembers.Length; i++)
            {
                if (partyMembers[i] == null || partyMembers[i].health <= 0)
                {
                    if (i < 3) frontRow.Remove(i);
                    else backRow.Remove(i);
                }
            }
            if (backRow.Count == 0 && frontRow.Count == 0)
            {
                Debug.LogError("Player didn't have any party members alive.");
                return null;
            }
            
            // Pick the enemies to use it on now
            List<int> attackingList = new List<int>();
            bool useBackRow = Random.Range(0, 1f) > backrowPercent;
            if (backRow.Count == 0)
                useBackRow = false;
            switch (selectionType)
            {
                case BattleEnemySelection.SelectionType.None:
                {
                    Debug.LogError("Tried to target enemies when the attack doesn't target anyone.");
                    break;
                }
                case BattleEnemySelection.SelectionType.Single:
                    // Randomly pick a single member
                    if (!useBackRow && frontRow.Count > 0)
                    {
                        // Randomly pick from front row
                        attackingList.Add(frontRow[Random.Range(0, frontRow.Count)]);
                    }
                    else if(backRow.Count > 0)
                    {
                        // Randomly pick from back row
                        attackingList.Add(backRow[Random.Range(0, backRow.Count)]);
                    }
                    else
                    {
                        Debug.LogError("Tried to target a row with no enemies in it.");
                    }
                    break;
                case BattleEnemySelection.SelectionType.AllRow:
                    // Decide which row to use
                    if (useBackRow)
                        attackingList = backRow;
                    else attackingList = frontRow;
                    break;
                case BattleEnemySelection.SelectionType.AllParty:
                    attackingList.AddRange(frontRow);
                    attackingList.AddRange(backRow);
                    break;
            }

            return attackingList.ToArray();
        }
    }
}