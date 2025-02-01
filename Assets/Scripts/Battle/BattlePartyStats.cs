using System;
using UnityEngine;

namespace Game.Battle
{
    public class BattlePartyStats : MonoBehaviour
    {
        [Serializable]
        public struct BuffEffect
        {
            public int currentIntensity;
            public int turnsToDecrease;
        }
        [Serializable]
        public struct BattlePartyStat
        {
            public bool isGuarding;
            public BuffEffect attackLevel;
            public BuffEffect defenseLevel;
            public BuffEffect accuracyLevel;
            public Status.Status status;
            public BuffEffect isCharging;
            public BuffEffect isFocusing;
        }

        public BattlePartyStat[] stats;

        public void Initialize(Entity.Entity[] partySlots)
        {
            stats = new BattlePartyStat[partySlots.Length];

            for (int i = 0; i < partySlots.Length; i++)
            {
                Entity.Entity entity = partySlots[i];
                
                if (entity == null)
                    continue;
                
                stats[i].status = entity.currentStatus;
            }
        }
    }
}