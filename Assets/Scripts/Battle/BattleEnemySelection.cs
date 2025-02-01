using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using Entity;
using Game.Audio;
using Game.Battle.UI;
using UnityEngine;

namespace Game.Battle
{
    public class BattleEnemySelection : BattleMenuScript
    {
        private int[] selectedEnemies;
        private SelectionType currentSelectionType = SelectionType.Single;
        private BattleController.Enemy[] enemies;
        private Dictionary<BattleController.Enemy, float> enemyFrontRanked;
        private Dictionary<BattleController.Enemy, float> enemyBackRanked;
        private SelectionType lastSelectionType = SelectionType.Single;
        private bool isSelecting;
        private BattleController battleController;
        private SFXController sfxController;
        private EnemySelectionStats selectionStats;
        [SerializeField] private BattleEnemySelector enemySelector;
        [SerializeField] private Animator cameraAnimator;
        private int currentRowSelected;

        private Element? currentElement;
        
        public enum SelectionType
        {
            None, Single, AllRow, AllParty
        }

        protected override void Awake()
        {
            currentElement = null;
            selectionStats = FindObjectOfType<EnemySelectionStats>();
            battleController = FindObjectOfType<BattleController>();
            enemyFrontRanked = new Dictionary<BattleController.Enemy, float>();
            enemyBackRanked = new Dictionary<BattleController.Enemy, float>();
            base.Awake();
        }

        protected override void InitializeButtonTexts()
        {
            // Do nothing
        }

        public override void Movement(Vector2 movement)
        {
            if (!isSelecting)
            {
                Debug.LogError("Enemy selection received movement when not opened.");
                return;
            }
            
            int selectedEnemy = 0;

            // Do cursor movement
            //Debug.Log("Movement = " + movement);
            
            if (movement.x != 0)
            {
                if (movement.x < 0)
                    selectedEnemy = findNextValidSelection(currentSelected, -1);
                else if (movement.x > 0) selectedEnemy = findNextValidSelection(currentSelected, 1);
            }

            else if (movement.y != 0)
            {
                int newVertEnemy = findNextRowSpecificSelection(currentSelected,
                    !battleController.IsInFrontRow(currentSelected));

                if (newVertEnemy != -1)
                {
                    selectedEnemy = newVertEnemy;
                }
                else
                {
                    // Couldn't find enemy to go to vertically, cancel this
                    return;
                }
            }
            
            PlaySound("ui_move");
            SetCursorTo(currentSelected, selectedEnemy);
            
            switch (currentSelectionType)
            {
                case SelectionType.Single:
                {
                    SelectDemon(selectedEnemy);
                    break;
                }
                case SelectionType.AllRow:
                {
                    // Make currentSelected = 0 for when selecting front row, and 1 for back
                    if (movement.y == 0) return;
                    SelectDemonRow(currentRowSelected == 0 ? 1 : 0, currentElement);
                    break;
                }
            }
        }

        public override void OnAccept()
        {
            isSelecting = false;
            selectionStats.SetVisible(false);
            cameraAnimator.Play("ZoomOut");
        }

        public override void OnClose()
        {
            enemySelector.HideSelection();
            enemySelector.HideAllResistanceIcons(enemies);
            base.OnClose();
        }
        
        public override void OnCancel()
        {
            enemySelector.HideSelection();
            selectionStats.SetVisible(false);
            PlaySound("ui_cancel");
            cameraAnimator.Play("ZoomOut");
        }

        public override void OnOpened()
        {
            cameraAnimator.Play("ZoomIn");
        }

        protected override void SelectCurrent()
        {
            
        }

        private void SetCursorTo(int lastSelection, int select)
        {
            if (currentSelectionType == SelectionType.Single)
            {
                if (enemies[lastSelection].demonObject != null)
                {
                    enemySelector.HideResistanceIcon(enemies[lastSelection]);
                    enemySelector.SetProjectorVisible(enemies[lastSelection], false);
                }
                if (enemies[select].demonObject != null)
                {
                    enemySelector.SetProjectorVisible(enemies[select], true);
                }
            }

            if (enemies[select].demonObject == null)
            {
                select = findFirstValidSelection();
            }

            if (select == -1)
            {
                Debug.LogError("Couldn't locate enemy. Are they all dead?");
            }
            enemySelector.SelectDemon(enemies[select]);
            selectionStats.SetVisible(true);
            selectionStats.LoadEntity(enemies[select].demonStats);
            currentSelected = select;
        }

        public int findFirstValidSelection()
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].demonObject != null)
                {
                    return i;
                }
            }

            return -1;
        }
        
        private void SelectDemon(int select)
        {
            enemySelector.ShowResistanceIcon(enemies[select], currentElement);
            selectedEnemies = new int[1];
            selectedEnemies[0] = select;
        }
        
        private void SelectDemonRow(int rowSelect, Element? element)
        {
            List<BattleController.Enemy> rowList = new List<BattleController.Enemy>();
            List<BattleController.Enemy> lastRow = new List<BattleController.Enemy>();

            for (int i = 0; i < battleController.GetEnemies().Length; i++)
            {
                if (battleController.GetEnemies()[i].demonObject == null) continue;
                
                if (i < battleController.GetMaxEnemiesInRow() / 2)
                {
                    if(rowSelect == 0)
                        rowList.Add(battleController.GetEnemies()[i]);
                    else lastRow.Add(battleController.GetEnemies()[i]);
                }else if (i >= battleController.GetMaxEnemiesInRow() / 2)
                {
                    if(rowSelect == 1)
                        rowList.Add(battleController.GetEnemies()[i]);
                    else lastRow.Add(battleController.GetEnemies()[i]);
                }
            }
            
            currentRowSelected = rowSelect;
            selectedEnemies = new int[rowList.Count];
            for (int i = 0; i < rowList.Count; i++)
            {
                selectedEnemies[i] = rowList[i].arrayID;
            }
            enemySelector.SelectDemon(enemies[currentSelected]);
            enemySelector.ShowResistanceIcon(rowList.ToArray(), element);
            enemySelector.HideAllResistanceIcons(lastRow.ToArray());
            enemySelector.SetProjectorVisible(lastRow.ToArray(), false);
            enemySelector.SetProjectorVisible(rowList.ToArray(), true);
        }

        private void SelectAllDemons(Element? element)
        {
            List<BattleController.Enemy> enemyList = new List<BattleController.Enemy>();

            for (int i = 0; i < battleController.GetEnemies().Length; i++)
            {
                if (battleController.GetEnemies()[i].demonObject == null) continue;
                enemyList.Add(enemies[i]);
            }
            
            selectedEnemies = new int[enemyList.Count];
            for (int i = 0; i < enemyList.Count; i++)
            {
                selectedEnemies[i] = enemyList[i].arrayID;
            }
            enemySelector.SelectDemon(enemies[currentSelected]);
            enemySelector.ShowResistanceIcon(enemyList.ToArray(), element);
            enemySelector.SetProjectorVisible(enemyList.ToArray(), true);
        }
        
        /// <summary>
        /// Finds the nearest enemy to the current int on the same row.
        /// Returns -1 if there were no enemies found.
        /// </summary>
        /// <param name="start">The position to start from.</param>
        /// <param name="frontRow">If we are checking the frontrow or the backrow.</param>
        /// <returns></returns>
        private int findNextRowSpecificSelection(int start, bool frontRow)
        {
            // Make sure there are even demons in the desired row to begin with
            List<int> currentRowEnemies = new List<int>();
            
            // Only add demons in the same row as the current selection
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].demonStats == null) continue;
                
                switch (frontRow)
                {
                    case true when i < battleController.GetMaxEnemiesInRow() / 2:
                    case false when i >= battleController.GetMaxEnemiesInRow() / 2:
                        if(enemies[i].demonObject != null)
                            currentRowEnemies.Add(i);
                        break;
                }
            }

            if (currentRowEnemies.Count == 0)
            {
                // No enemies in desired row, cancel
                return -1;
            }
            
            
            // If going from front to back, add maxrownumber
            // If going from back to front, sub maxrownumber
            if (frontRow)
                start -= battleController.GetMaxEnemiesInRow();
            else
                start += battleController.GetMaxEnemiesInRow();
            
            int result = -1;
            int ascendingCheck = start, descendingCheck = start;
            bool checkAsc = true, checkDesc = true;
            while (true)
            {
                // Check to make sure each value is still in the range of the row
                if (frontRow)
                {
                    // We should be within 0 and maxenemyrow
                    if (descendingCheck < 0)
                    {
                        // Invalid now
                        checkDesc = false;
                    }
                    if (ascendingCheck > battleController.GetMaxEnemiesInRow())
                    {
                        checkAsc = false;
                    }
                }
                else
                {
                    // We should be within maxenemyrow and maxenemyrow*2
                    if (descendingCheck < battleController.GetMaxEnemiesInRow())
                    {
                        // Invalid now
                        checkDesc = false;
                    }
                    if (ascendingCheck > battleController.GetMaxEnemiesInRow() * 2)
                    {
                        checkAsc = false;
                    }
                }

                // Check for entity
                if (checkAsc)
                {
                    if (ascendingCheck < 0)
                        ascendingCheck = 0;
                    if (ascendingCheck < enemies.Length && enemies[ascendingCheck].demonStats != null)
                    {
                        result = ascendingCheck;
                        break;
                    }
                    
                    ascendingCheck++;
                }

                if (checkDesc)
                {
                    if (descendingCheck >= enemies.Length)
                        descendingCheck = enemies.Length - 1;
                    if (descendingCheck >= 0 && enemies[descendingCheck].demonStats != null)
                    {
                        result = descendingCheck;
                        break;
                    }
                    
                    descendingCheck--;
                }

                if (!checkAsc && !checkDesc)
                {
                    Debug.LogError("Couldn't find an enemy to go to");
                    break;
                }
            }

            return result;
        }

        private int findNextValidSelection(int start, int direction)
        {
            // Choose array
            List<int> currentRowEnemies = new List<int>();
            bool useFront = start < battleController.GetMaxEnemiesInRow() / 2;
            
            // Only add demons in the same row as the current selection
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].demonStats == null) continue;
                
                switch (useFront)
                {
                    case true when i < battleController.GetMaxEnemiesInRow() / 2:
                    case false when i >= battleController.GetMaxEnemiesInRow() / 2:
                        currentRowEnemies.Add(i);
                        break;
                }
            }

            if (currentRowEnemies.Count == 0)
            {
                // There are no enemies in this row. Reset to the first one
                return findFirstValidSelection();
            }

                // Get current array position of the existing selection
            int currentSelectionID = currentRowEnemies.IndexOf(start);

            int returnVal = -1;
            if (currentSelectionID + direction < 0)
            {
                // Wrap to the end of array
                returnVal = currentRowEnemies[^1];
            }

            else if (currentSelectionID + direction > currentRowEnemies.Count - 1)
            {
                // Wrap to beginning
                returnVal = currentRowEnemies[0];
            }
            
            // No need to wrap, add
            else returnVal = currentRowEnemies[currentSelectionID + direction];
            string itemsInArray = "";
            foreach (int i in currentRowEnemies)
            {
                itemsInArray += ", " + i;
            }
            
            //Debug.Log("Trying to select demon at " + returnVal + " with a start of " + start + "\n" + itemsInArray + ", with an array size of " + currentRowEnemies.Count);
            return returnVal;
        }
        
        public IEnumerator StartSelection(BattleController.Enemy[] enemiesRowed, SelectionType selectionType, int selectionStart, Element? elementType)
        {
            currentElement = elementType;
            bool resetSelection = false;
            enemies = enemiesRowed;

            if (selectionType != lastSelectionType)
            {
                // Reset selectionStart
                resetSelection = true;
            }
            else
            {
                // Make sure the entity is still alive, if not then reset
                if (enemies[selectionStart].demonStats == null) 
                { 
                    resetSelection = true;
                }
            }

            if (resetSelection)
            {
                if (selectionType == SelectionType.Single)
                {
                    bool foundEnemy = false;
                    for (int i = 0; i < enemiesRowed.Length; i++)
                    {
                        if (enemiesRowed[i].demonStats != null)
                        {
                            selectionStart = i;
                            foundEnemy = true;
                            break;
                        }
                    }

                    if (!foundEnemy)
                    {
                        Debug.LogError("Couldn't find an enemy to target.");
                        yield break;
                    }
                    
                    // Start selection
                    SelectDemon(selectionStart);
                }
                else if (selectionType == SelectionType.AllRow)
                {
                    // Default to front row
                    SelectDemonRow(0, elementType);
                }
                else if (selectionType == SelectionType.AllParty)
                {
                    // No need to calcualte anything here
                    SelectAllDemons(elementType);
                }
            }
            currentSelectionType = selectionType;
            
            // DONE
            lastSelectionType = selectionType;
            isSelecting = true;
            
            // Check if previously selected demon is still alive
            if (enemies[currentSelected].demonObject == null)
            {
                // Demon doesn't exist anymore, change position
                currentSelected = currentSelected == -1 ? findFirstValidSelection() : findNextValidSelection(currentSelected, 1);
                
                if (currentSelected == -1)
                    currentSelected = findFirstValidSelection();
            }
            else
            {
                enemySelector.SelectDemon(enemies[currentSelected]);
                enemySelector.ShowResistanceIcon(enemies[currentSelected], currentElement);
            }
            
            SetCursorTo(currentSelected, selectionStart);
            
            while (isSelecting)
            {
                yield return null;
            }

            if (selectionType == SelectionType.Single)
            {
                selectedEnemies = new int[1];
                selectedEnemies[0] = currentSelected;
            }
            enemySelector.SetProjectorVisible(enemies, false);
            currentElement = null;
        }

        public void ResetSelection()
        {
            selectedEnemies = Array.Empty<int>();
        }

        public int[] GetEntitiesSelected()
        {
            return selectedEnemies;
        }

        public bool IsSelecting() => isSelecting;
    }
}