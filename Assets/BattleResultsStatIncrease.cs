using System.Collections;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Battle
{
    public class BattleResultsStatIncrease : MonoBehaviour
    {
        [SerializeField] private BattleResultsStatIncreaseObject[] increaseObjects;
        [SerializeField] private PlayerInputWrapper playerInput;
        private int currentSelectedMenu = 0;
        private bool hasPlacedAllPoints;
        private int totalPoints;
        private int remainingPoints;
        
        private void Awake()
        {
            playerInput.GetMovementAction().performed += MenuMovement;
        }

        public IEnumerator ExecuteStatScreen(Entity.Entity entity, int totalPoints)
        {
            this.totalPoints = totalPoints;
            remainingPoints = totalPoints;
            increaseObjects[0].SetStatAmount(entity.stats.strength);
            increaseObjects[1].SetStatAmount(entity.stats.intelligence);
            increaseObjects[2].SetStatAmount(entity.stats.magic);
            increaseObjects[3].SetStatAmount(entity.stats.vitality);
            increaseObjects[4].SetStatAmount(entity.stats.agility);
            increaseObjects[5].SetStatAmount(entity.stats.luck);

            while (true)
            {
                yield return null;
            }
        }

        private void MenuMovement(InputAction.CallbackContext ctx)
        {
            if (!hasPlacedAllPoints)
            {
                Vector2 movement = ctx.ReadValue<Vector2>();

                if (movement.x != 0)
                {
                    // Player is adding or removing a point
                    
                    if (movement.x > 0)
                    {
                        // Player is adding a point allocation
                        if (remainingPoints <= 0)
                            return;
                        
                        remainingPoints--;
                        increaseObjects[currentSelectedMenu].ChangeAmount(1);
                    }
                    else
                    {
                        // Player is removing a point allocation
                        
                        // Make sure we have points allocated to position
                        if (increaseObjects[currentSelectedMenu].GetPointsAdded() > 0)
                        {
                            // Player can remove a point
                            remainingPoints++;
                            increaseObjects[currentSelectedMenu].ChangeAmount(-1);
                        }
                    }
                    
                    // Check if player has placed all points
                    if (remainingPoints <= 0)
                    {
                        // Player has placed all points
                        hasPlacedAllPoints = true;
                    }
                    
                }
                
                else if (movement.y != 0)
                {
                    // Digitize input
                    int input = 0;
                    if (movement.y > 0)
                        input = -1;
                    else input = 1;
                    
                    // Player is moving up or down
                    if (input == 1 && currentSelectedMenu >= increaseObjects.Length - 1)
                    {
                        // Player was moving down and is at the bottom of the list
                        ChangeMenuSelection(0);
                    }
                    else if (input == -1 && currentSelectedMenu <= 0)
                    {
                        // Player was moving up and is at the top of the list
                        ChangeMenuSelection(increaseObjects.Length - 1);
                    }
                    else
                    {
                        // Just move value normally
                        ChangeMenuSelection(currentSelectedMenu + input);
                    }
                }
            }
            else
            {
                // Player is moving with all skill points placed
            }
        }

        private void ChangeMenuSelection(int newSelection)
        {
            int lastSelected = currentSelectedMenu;
            int newSelected = newSelection;
            // Deselect old selection
            increaseObjects[currentSelectedMenu].SetSelected(false);
            
            // Select new
            currentSelectedMenu = newSelection;
            increaseObjects[currentSelectedMenu].SetSelected(true);
            Debug.Log("Unselecting " + lastSelected + ", selecting " + newSelected);
        }
        
    }
}