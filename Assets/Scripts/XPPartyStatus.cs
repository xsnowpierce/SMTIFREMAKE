using System.Collections;
using Database;
using Game.Battle.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class XPPartyStatus : MonoBehaviour
    {
        [SerializeField] private PartyStatus partyStatus;
        [SerializeField] private GameObject xpBarDisable;
        [SerializeField] private TMP_Text xpText;
        [SerializeField] private Image xpBar;
        [SerializeField] private Animator levelupAnimator;
        [SerializeField] private Animator animator;
        private bool isIncreasingXP;
        private BattleResults battleResults;
        private Entity.Entity currentEntity;
        private bool hasLevelUp;

        public bool GetHasLevelUp() => hasLevelUp;

        public Entity.Entity GetEntity() => currentEntity;

        public void SetupMember(Entity.Entity entity, Databases databases)
        {
            xpBarDisable.SetActive(true);
            xpBar.fillAmount = entity.currentXP * 1f / entity.GetMaxXP();
            currentEntity = entity;
            partyStatus.SetupMember(entity, databases);
            if(entity.health <= 0)
                partyStatus.SetDeadState(true);
        }

        public void IncreaseXP(int gainXP, float increaseSpeed, BattleResults results)
        {
            if (partyStatus.GetCurrentEntity() == null) return;
            battleResults = results;
            isIncreasingXP = true;
            xpText.text = "+<color=#76d3f5>" + gainXP;
            StartCoroutine(xpIncrease(gainXP, increaseSpeed));
        }

        private IEnumerator xpIncrease(int gainXP, float increaseSpeed)
        {
            if (partyStatus.GetCurrentEntity().health <= 0)
            {
                // entity is dead, don't add xp
                isIncreasingXP = false;
                yield break;
            }
            
            //Debug.Log("Trying to add " + gainXP + " to " + partyStatus.GetCurrentEntity().entityName + ", with a max XP of " + partyStatus.GetCurrentEntity().GetMaxXP());
            
            int levelupTimes = 0;
            while (gainXP > partyStatus.GetCurrentEntity().GetMaxXP())
            {
                gainXP -= partyStatus.GetCurrentEntity().GetMaxXP();
                levelupTimes++;
            }

            if (levelupTimes > 0)
                hasLevelUp = true;
            
            //Debug.Log(partyStatus.GetCurrentEntity().entityName + " is levelling up " + levelupTimes + " times.");

            for (int i = 0; i < levelupTimes; i++)
            {
                // TODO speed to end of xp bar and do levelup animation
                while (xpBar.fillAmount < 1)
                {
                    xpBar.fillAmount += (1 - (levelupTimes / 10f)) * Time.deltaTime;
                    yield return null;
                }
                
                // TODO Do levelup animation
                yield return StartCoroutine(LevelUpAnim());
                
                xpBar.fillAmount = 0f;
            }

            // Just fill the xp bar up with current xp
            float targetAmount = (float) gainXP / partyStatus.GetCurrentEntity().GetMaxXP();
            float amountToGain = targetAmount - xpBar.fillAmount;
            float speed = amountToGain * increaseSpeed;
            
            while (xpBar.fillAmount < targetAmount)
            {
                xpBar.fillAmount += speed * Time.deltaTime;
                yield return null;
            }
            xpBar.fillAmount = targetAmount;
            
            isIncreasingXP = false;
        }

        private IEnumerator LevelUpAnim()
        {
            battleResults.PlayLevelupSound();
            levelupAnimator.Play("levelUp");
            yield return new WaitForSeconds(1f);
            levelupAnimator.Play("levelupEnd");
            yield return new WaitForSeconds(0.17f);
        }

        public void PlayOpenAnimation()
        {
            animator.Play("ResultsPartyStatusAppear");
        }

        public void PlayCloseAnimation()
        {
            animator.Play("ResultsPartyStatusDisappear");
        }

        public bool GetIsIncreasingXP() => isIncreasingXP;

        public void SetupEmpty()
        {
            xpBarDisable.SetActive(false);
            partyStatus.SetupEmpty();
            currentEntity = null;
        }

        public PartyStatus GetPartyStatus()
        {
            return partyStatus;
        }

        public IEnumerator increaseXPBar()
        {
            yield return null;
        }
    }
}