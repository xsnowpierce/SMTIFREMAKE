using System;
using Items;
using Items.Scripts;
using UnityEngine;

namespace Entity
{
    [CreateAssetMenu(fileName = "New Party Character", menuName = "Entity/Party Character")]
    public class Human : Entity
    {
        [Serializable]
        public struct Equipment
        {
            public Armor helmet;
            public Armor body;
            public Armor arms;
            public Armor legs;
            public MeleeWeapon meleeWeapon;
            public FirearmWeapon firearmWeapon;
            public Bullet bullet;
        }
        
        [Header("Party Settings")]
        public Equipment equipment;
        public Demon currentGuardian;

        public override int GetAttackPower()
        {
            int str = stats.strength;
            int meleeExtra = 0;
            
            if (equipment.helmet != null)
            {
                str += equipment.helmet.bonuses.strengthBonus;
            }
            if (equipment.body != null)
            {
                str += equipment.body.bonuses.strengthBonus;
            }
            if (equipment.arms != null)
            {
                str += equipment.arms.bonuses.strengthBonus;
            }
            if (equipment.legs != null)
            {
                str += equipment.legs.bonuses.strengthBonus;
            }
            if (equipment.meleeWeapon != null)
            {
                str += equipment.meleeWeapon.bonuses.strengthBonus;
                meleeExtra = equipment.meleeWeapon.hit;
            }
            
            return (level / 4) + str + meleeExtra;
        }

        public override int GetAttackAccuracy()
        {
            int agl = stats.agility;
            int lck = stats.luck;
            
            if (equipment.helmet != null)
            {
                agl += equipment.helmet.bonuses.agilityBonus;
                lck += equipment.helmet.bonuses.luckBonus;
            }
            if (equipment.body != null)
            {
                agl += equipment.body.bonuses.agilityBonus;
                lck += equipment.body.bonuses.luckBonus;
            }
            if (equipment.arms != null)
            {
                agl += equipment.arms.bonuses.agilityBonus;
                lck += equipment.arms.bonuses.luckBonus;
            }
            if (equipment.legs != null)
            {
                agl += equipment.legs.bonuses.agilityBonus;
                lck += equipment.legs.bonuses.luckBonus;
            }
            if (equipment.meleeWeapon != null)
            {
                agl += equipment.meleeWeapon.bonuses.agilityBonus;
                lck += equipment.meleeWeapon.bonuses.luckBonus;
            }
            
            return level + lck / 2 + agl + equipment.meleeWeapon.hit;
        }

        public int GetGunAttackPower()
        {
            return level / 4 + (equipment.firearmWeapon.attackDamage + equipment.bullet.attackDamage);
        }

        public int GetGunAccuracy()
        {
            int agl = stats.agility;
            int lck = stats.luck;
            
            if (equipment.helmet != null)
            {
                agl += equipment.helmet.bonuses.agilityBonus;
                lck += equipment.helmet.bonuses.luckBonus;
            }
            if (equipment.body != null)
            {
                agl += equipment.body.bonuses.agilityBonus;
                lck += equipment.body.bonuses.luckBonus;
            }
            if (equipment.arms != null)
            {
                agl += equipment.arms.bonuses.agilityBonus;
                lck += equipment.arms.bonuses.luckBonus;
            }
            if (equipment.legs != null)
            {
                agl += equipment.legs.bonuses.agilityBonus;
                lck += equipment.legs.bonuses.luckBonus;
            }
            if (equipment.meleeWeapon != null)
            {
                agl += equipment.meleeWeapon.bonuses.agilityBonus;
                lck += equipment.meleeWeapon.bonuses.luckBonus;
            }
            
            return level + lck / 2 + agl + equipment.firearmWeapon.hit;
        }

        public override int GetDefense()
        {
            int vit = stats.vitality;
            int agl = stats.agility;
            int extraDefense = 0;
            if (equipment.helmet != null)
            {
                agl += equipment.helmet.bonuses.agilityBonus;
                vit += equipment.helmet.bonuses.vitalityBonus;
                extraDefense += equipment.helmet.defense;
            }
            if (equipment.body != null)
            {
                agl += equipment.body.bonuses.agilityBonus;
                vit += equipment.body.bonuses.vitalityBonus;
                extraDefense += equipment.body.defense;
            }
            if (equipment.arms != null)
            {
                agl += equipment.arms.bonuses.agilityBonus;
                vit += equipment.arms.bonuses.vitalityBonus;
                extraDefense += equipment.arms.defense;
            }
            if (equipment.legs != null)
            {
                agl += equipment.legs.bonuses.agilityBonus;
                vit += equipment.legs.bonuses.vitalityBonus;
                extraDefense += equipment.legs.defense;
            }
            return ((vit + agl) / 2) + extraDefense;
        }

        public override int GetEvasion()
        {
            int agl = stats.agility;
            int itl = stats.strength;
            int lck = stats.luck;
            int evasion = 0;
            
            if (equipment.helmet != null)
            {
                agl += equipment.helmet.bonuses.agilityBonus;
                itl += equipment.helmet.bonuses.intelligenceBonus;
                lck += equipment.helmet.bonuses.luckBonus;
                evasion += equipment.helmet.evasion;
            }
            if (equipment.body != null)
            {
                agl += equipment.body.bonuses.agilityBonus;
                itl += equipment.body.bonuses.intelligenceBonus;
                lck += equipment.body.bonuses.luckBonus;
                evasion += equipment.body.evasion;
            }
            if (equipment.arms != null)
            {
                agl += equipment.arms.bonuses.agilityBonus;
                itl += equipment.arms.bonuses.intelligenceBonus;
                lck += equipment.arms.bonuses.luckBonus;
                evasion += equipment.arms.evasion;
            }
            if (equipment.legs != null)
            {
                agl += equipment.legs.bonuses.agilityBonus;
                itl += equipment.legs.bonuses.intelligenceBonus;
                lck += equipment.legs.bonuses.luckBonus;
                evasion += equipment.legs.evasion;
            }

            return level + (itl + lck) / 4 + agl + evasion;
        }

        public override int GetMagicPower()
        {
            return (Mathf.FloorToInt((level * 2f + stats.intelligence + stats.magic) / 4) / 2) + 4;
        }
    }
}