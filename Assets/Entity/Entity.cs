using System;
using Entity.Skills;
using Game;
using UnityEngine;

namespace Entity
{
    public enum ResistanceType
    {
        Normal,
        Resist,
        Null,
        Weak,
        Repel,
        Absorb
    }

    [Serializable]
    public struct EntityStats
    {
        public int strength;
        public int intelligence;
        public int magic;
        public int vitality;
        public int agility;
        public int luck;
    }
    
    public abstract class Entity : ScriptableObject
    {
        [Header("Info")]
        public string entityName;
        public DemonRace race;
        
        [Header("Sprite Settings")] 
        public Sprite[] sprites;
        public float spriteSpeed = 1f;
        public float spriteHeight;
        
        [Header("Stats")]
        public int level;
        public int health;
        public int mana;
        public int currentXP;
        public Skill[] skills;
        public Status.Status currentStatus = Status.Status.Normal;
        [Space(10)] 
        public EntityStats stats;
        
        [Header("Elements")] 
        public ElementResistances Resistances = new()
        {
            almighty = ResistanceType.Normal, curse = ResistanceType.Normal, 
            electricity = ResistanceType.Normal, expel = ResistanceType.Normal, 
            fire = ResistanceType.Normal, force = ResistanceType.Normal, gun = ResistanceType.Normal, 
            ice = ResistanceType.Normal, nerve = ResistanceType.Normal, physical = ResistanceType.Normal
        };

        [Header("Ailment Settings")] 
        public ResistanceType magical;
        public ResistanceType bind;
        
        // FORMULAS

        public virtual int GetMaxHP()
        {
            return BattleFormuals.GetMaxHP(level, stats.vitality);
        }
        
        public virtual int GetMaxMana()
        {
            return BattleFormuals.GetMaxMana(level, stats.magic);
        }

        public virtual int GetMaxXP()
        {
            return BattleFormuals.GetMaxXP(this);
        }

        public abstract int GetAttackPower();
        public abstract int GetAttackAccuracy();
        public abstract int GetDefense();
        public abstract int GetEvasion();

        public virtual int GetMagicPower()
        {
            return BattleFormuals.GetMagicPower(stats.intelligence, stats.magic);
        }
        
        public virtual int GetMagicAccuracy()
        {
            return BattleFormuals.GetMagicAccuracy(stats.intelligence, stats.magic);
        }

        public ResistanceType GetAffinityToElement(Element element)
        {
            switch (element)
            {
                case Element.Physical:
                    return Resistances.physical;
                case Element.Gun:
                    return Resistances.gun;
                case Element.Fire:
                    return Resistances.fire;
                case Element.Ice:
                    return Resistances.ice;
                case Element.Electric:
                    return Resistances.electricity;
                case Element.Force:
                    return Resistances.force;
                case Element.Nerve:
                    return Resistances.nerve;
                case Element.Expel:
                    return Resistances.expel;
                case Element.Curse:
                    return Resistances.curse;
                case Element.Almighty:
                    return Resistances.almighty;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, null);
            }
        }
    }
}