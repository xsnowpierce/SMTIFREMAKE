using System;
using Entity;
using UnityEngine;

namespace Data
{
    [Serializable]
    public struct CurrentEquipment
    {
        
    }
    public class CharacterPartyMemberStats : PartyMemberStats
    {
        [Header("Character Stats")]
        public Demon currentGuardian;
        public int currentXp;
        public CurrentEquipment equipment;
        [Header("Upgradable Stats")]
        public int strength;
        public int intelligence;
        public int magic;
        public int vitality;
        public int agility;
        public int luck;
        [Header("Other Stats")]
        public int attack;
        public int accuracy;
        public int defense;
        public int evasion;
        public int magicAttack;
        public int magicHitRate;
    }
}