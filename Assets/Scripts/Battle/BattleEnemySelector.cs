using System.Collections.Generic;
using Battle;
using Entity;
using UnityEngine;

namespace Game.Battle
{
    public class BattleEnemySelector : MonoBehaviour
    {
        [SerializeField] private Transform disableObject;

        private void Awake()
        {
            HideSelection();
        }
        
        
        public void SelectDemon(BattleController.Enemy enemy)
        {
            Vector3 demonPosition = enemy.demonObject.position;
            transform.position = demonPosition;
            disableObject.gameObject.SetActive(true);
        }

        public void ShowResistanceIcon(BattleController.Enemy enemy, Element? element)
        {
            if (element != null && enemy.spriteLoader != null)
            {
                enemy.spriteLoader.ShowResistanceIcon(element);
            }
        }
        
        public void ShowResistanceIcon(BattleController.Enemy[] enemies, Element? element)
        {
            foreach (BattleController.Enemy enemy in enemies)
            {
                ShowResistanceIcon(enemy, element);
            }
        }

        public void HideResistanceIcon(BattleController.Enemy enemy)
        {
            if (enemy.spriteLoader != null)
            {
                enemy.spriteLoader.HideResistanceIcon();
            }
        }

        public void SetProjectorVisible(BattleController.Enemy enemy, bool value)
        {
            if (enemy.spriteLoader != null)
            {
                enemy.spriteLoader.SetProjectorVisible(value);
            }
        }

        public void SetProjectorVisible(BattleController.Enemy[] enemies, bool value)
        {
            foreach (BattleController.Enemy enemy in enemies)
            {
                SetProjectorVisible(enemy, value);
            }
        }

        public void HideAllResistanceIcons(BattleController.Enemy[] enemies)
        {
            foreach (BattleController.Enemy enemy in enemies)
            {
                if (enemy.spriteLoader != null)
                {
                    enemy.spriteLoader.HideResistanceIcon();
                }
            }
        }

        public void HideSelection()
        {
            disableObject.gameObject.SetActive(false);
        }
    }
}