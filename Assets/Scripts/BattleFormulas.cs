using System;
using System.Collections.Generic;
using Battle;
using Data;
using Entity;
using Entity.Skills;
using Entity.Skills.Physical;
using Entity.Skills.Spells;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game
{
    public static class BattleFormuals
    {

        public static readonly float ITEM_DROP_RATE = 0.35f;
        
        public static int GetMaxHP(int level, int vitality)
        {
            float vitInfluence = (vitality * 4);
            float levelInfluence = 1.35f * (7 * level);
            
            return Mathf.CeilToInt(vitInfluence + levelInfluence);
        }

        public static int GetMaxMana(int level, int magic)
        {
            return Mathf.CeilToInt((level * 1.3f) + (magic * 1f) * 4f);
        }

        public static int GetMagicPower(int intelligence, int magic)
        {
            return intelligence / 4 + magic;
        }

        public static int GetMagicAccuracy(int intelligence, int magic)
        {
            return magic / 4 + intelligence;
        }

        public static int CalculateDamage(Entity.Entity attacker, Entity.Entity defender, Skill attack, bool defenderGuarding)
        {
            //Mathf.RoundToInt(Mathf.Clamp((battleMoves[entityOrdered.partyID].basePower -
            // enemies[enemy].demonStats.GetDefense()) * randomizedMultiplier, 0, 9999));
            float basePower = 0f;
            float affinityChange = 0f;
            if (attack is PhysicalSkill physicalSkill)
            {
                basePower = physicalSkill.attackPower + (attacker.stats.strength * 1.51f);
                affinityChange = GetAffinityChange(Element.Physical, defender);
                
            }
            else if (attack is MagicSpell magicSpell)
            {
                basePower = magicSpell.attackPower + (attacker.stats.magic * 1.51f);
                affinityChange = GetAffinityChange(magicSpell.element, defender);
            }
            
            basePower += (basePower * affinityChange);
            if (defenderGuarding) basePower /= 2f;
            
            // Randomize element
            basePower = Mathf.CeilToInt(basePower * Random.Range(0.85f, 1.15f));

            return Mathf.Clamp(Mathf.CeilToInt(basePower), 1, 9999);
        }
        
        public static float GetAffinityChange(Element element, Entity.Entity entity)
        {
            float resistResistanceChange = 0.5f;
            float weakResistanceChange = 2f;
            
            switch (element)
            {
                case Element.Physical:
                    switch (entity.Resistances.physical)
                    {
                        case ResistanceType.Normal:
                            return 0;
                        case ResistanceType.Resist:
                            return resistResistanceChange;
                        case ResistanceType.Weak:
                            return weakResistanceChange;
                        default:
                            return 0;
                    }
                case Element.Gun:
                    switch (entity.Resistances.gun)
                    {
                        case ResistanceType.Normal:
                            return 0;
                        case ResistanceType.Resist:
                            return resistResistanceChange;
                        case ResistanceType.Weak:
                            return weakResistanceChange;
                        default:
                            return 0;
                    }
                case Element.Fire:
                    switch (entity.Resistances.fire)
                    {
                        case ResistanceType.Normal:
                            return 0;
                        case ResistanceType.Resist:
                            return resistResistanceChange;
                        case ResistanceType.Weak:
                            return weakResistanceChange;
                        default:
                            return 0;
                    }
                case Element.Ice:
                    switch (entity.Resistances.ice)
                    {
                        case ResistanceType.Normal:
                            return 0;
                        case ResistanceType.Resist:
                            return resistResistanceChange;
                        case ResistanceType.Weak:
                            return weakResistanceChange;
                        default:
                            return 0;
                    }
                case Element.Electric:
                    switch (entity.Resistances.electricity)
                    {
                        case ResistanceType.Normal:
                            return 0;
                        case ResistanceType.Resist:
                            return resistResistanceChange;
                        case ResistanceType.Weak:
                            return weakResistanceChange;
                        default:
                            return 0;
                    }
                case Element.Force:
                    switch (entity.Resistances.force)
                    {
                        case ResistanceType.Normal:
                            return 0;
                        case ResistanceType.Resist:
                            return resistResistanceChange;
                        case ResistanceType.Weak:
                            return weakResistanceChange;
                        default:
                            return 0;
                    }
                case Element.Nerve:
                    switch (entity.Resistances.nerve)
                    {
                        case ResistanceType.Normal:
                            return 0;
                        case ResistanceType.Resist:
                            return resistResistanceChange;
                        case ResistanceType.Weak:
                            return weakResistanceChange;
                        default:
                            return 0;
                    }
                case Element.Expel:
                    switch (entity.Resistances.expel)
                    {
                        case ResistanceType.Normal:
                            return 0;
                        case ResistanceType.Resist:
                            return resistResistanceChange;
                        case ResistanceType.Weak:
                            return weakResistanceChange;
                        default:
                            return 0;
                    }
                case Element.Curse:
                    switch (entity.Resistances.curse)
                    {
                        case ResistanceType.Normal:
                            return 0;
                        case ResistanceType.Resist:
                            return resistResistanceChange;
                        case ResistanceType.Weak:
                            return weakResistanceChange;
                        default:
                            return 0;
                    }
                case Element.Almighty:
                    switch (entity.Resistances.almighty)
                    {
                        case ResistanceType.Normal:
                            return 0;
                        case ResistanceType.Resist:
                            return resistResistanceChange;
                        case ResistanceType.Weak:
                            return weakResistanceChange;
                        default:
                            return 0;
                    }
                case Element.Recovery:
                    break;
                case Element.Support:
                    break;
                case Element.Other:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, null);
            }
            
            // Shouldn't get here but oh well ig
            return 0;
        }

        public static int GetGainedMacca(List<Entity.Entity> demonsDefeated)
        {
            int gainedMacca = 0;
            foreach (Entity.Entity enemy in demonsDefeated)
            {
                gainedMacca += Mathf.CeilToInt(enemy.GetMaxHP() * 0.491f);
            }
            return gainedMacca;
        }
        
        public static int[] GetPartyXPGain(List<Entity.Entity> demonsDefeated, SaveData saveData)
        {
            int[] xpEachMember = new int[saveData.partySlots.Length];
            for (int i = 0; i < xpEachMember.Length; i++)
            {
                Entity.Entity entity = saveData.GetPartyMembers()[i];
                if(entity != null)
                    xpEachMember[i] = GetXPGain(entity, demonsDefeated);
            }
            return xpEachMember;
        }

        public static int[] GetStockXPGain(List<Entity.Entity> demonsDefeated, SaveData data)
        {
            int[] xpEachMember = new int[data.entityStock.Length];
            
            for (int i = 0; i < data.entityStock.Length; i++)
            {
                bool inParty = false;
                foreach (int t in data.partySlots)
                {
                    if (i != t) continue;

                    // Is in party
                    inParty = true;
                    xpEachMember[i] = -1;
                    break;
                }

                if (inParty) continue;

                Entity.Entity entity = data.entityStock[i];
                if (entity is Human) continue;

                // Divide by 2 since they are part of stock
                xpEachMember[i] = Mathf.CeilToInt(GetXPGain(entity, demonsDefeated) / 2f);
            }

            return xpEachMember;
        }

        private static int GetXPGain(Entity.Entity entity, List<Entity.Entity> demonsDefeated)
        {
            if (entity == null) return 0;
            if (entity.health <= 0)
                return 0;
            
            int totalXP = 0;
            
            foreach (Entity.Entity enemy in demonsDefeated)
            {
                float levelDifferenceModifier = 1f;
                int levelDifference = entity.level - enemy.level;
                int levelDifferenceFormula = levelDifference * (levelDifference / 32);
                float xpFormula = enemy.level * 3.15f;
                
                if (levelDifference != 0)
                {
                    if (levelDifference is >= 10 or <= -10)
                        levelDifferenceModifier = 4f;
                    else levelDifferenceModifier = 1 + levelDifferenceFormula;
                }

                int gainedXP;
                switch (levelDifference)
                {
                    case < 0:
                        gainedXP = (int) (xpFormula * levelDifferenceModifier);
                        break;
                    case 0:
                        gainedXP = (int) (xpFormula);
                        break;
                    default:
                        gainedXP = (int) (xpFormula / levelDifferenceModifier);
                        break;
                }
                
                float randomRange = Random.Range(0.95f, 1.05f);
                gainedXP = (int) (gainedXP * randomRange);
                totalXP += gainedXP;
            }
            return totalXP;
        }

        public static int GetGainedMagnetite(List<Entity.Entity> demonsDefeated)
        {
            int gainedMag = 0;
            foreach (Entity.Entity enemy in demonsDefeated)
            {
                gainedMag += Mathf.CeilToInt(enemy.GetMaxHP() * 0.121f);
            }
            return gainedMag;
        }

        public static int GetMaxXP(Entity.Entity entity)
        {
            return Mathf.CeilToInt((3.52f * ((float) entity.level / 10)) * 12f) + 10;
        }
    }
}