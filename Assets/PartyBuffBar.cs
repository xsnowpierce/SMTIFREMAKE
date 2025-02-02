using System;
using UnityEngine;

namespace Game.Battle.UI
{
    public class PartyBuffBar : MonoBehaviour
    {
        [SerializeField] private RectTransform iconParent;
        [SerializeField] private GameObject iconPrefab;
        [Header("Icons")] 
        [SerializeField] private Sprite attackSprite;
        [SerializeField] private Sprite defenseSprite;
        [SerializeField] private Sprite evasionSprite;
        [SerializeField] private Sprite raisedSprite;
        [SerializeField] private Sprite superRaisedSprite;
        [SerializeField] private Sprite loweredSprite;
        [SerializeField] private Sprite superLoweredSprite;

        private void Awake()
        {
            BattlePartyStats.BattlePartyStat stat = new BattlePartyStats.BattlePartyStat();
            stat.attackLevel.currentIntensity = 1;
            stat.defenseLevel.currentIntensity = -2;
            SetCurrentBuffs(stat);
        }

        public void SetCurrentBuffs(BattlePartyStats.BattlePartyStat buffStatus)
        {
            KillChildren();

            if (buffStatus.attackLevel.currentIntensity != 0)
                CreateBuff(buffStatus.attackLevel, attackSprite);
            if (buffStatus.defenseLevel.currentIntensity != 0)
                CreateBuff(buffStatus.defenseLevel, defenseSprite);
            if (buffStatus.accuracyLevel.currentIntensity != 0)
                CreateBuff(buffStatus.accuracyLevel, evasionSprite);
        }

        private void CreateBuff(BattlePartyStats.BuffEffect effect, Sprite icon)
        {
            if (effect.currentIntensity != 0)
            {
                int intensity = effect.currentIntensity;
                int turnsLeft = effect.turnsToDecrease;

                GameObject newObj = Instantiate(iconPrefab, iconParent);
                PartyBuffIcon buffIcon = newObj.GetComponent<PartyBuffIcon>();

                Sprite iconToUse = intensity switch
                {
                    1 => raisedSprite,
                    2 => superRaisedSprite,
                    -1 => loweredSprite,
                    -2 => superLoweredSprite,
                    _ => null
                };

                buffIcon.SetIcons(icon, iconToUse);

                buffIcon.SetFading(turnsLeft == 1);
            }
        }

        private void KillChildren()
        {
            for (int i = 0; i < iconParent.childCount; i++)
            {
                Destroy(iconParent.GetChild(i));
            }
        }
    }
}