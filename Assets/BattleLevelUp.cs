using System.Collections;
using Database;
using Dialogue;
using Entity;
using Game.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.Battle.UI
{
    public class BattleLevelUp : MonoBehaviour
    {
        [SerializeField] private GameObject disableObject;
        [SerializeField] private Image entitySprite;
        [SerializeField] private BattleChangeSelectedUI selectedUI;
        [SerializeField] private BattleLevelupEquipment levelupEquipment;
        [SerializeField] private BattleDialogueController battleDialogue;
        [SerializeField] private BattleResultsStatIncrease battleResultsStatIncrease;

        private Databases database;
        private PlayerInputWrapper playerInput;

        public IEnumerator LevelUp(Entity.Entity entity, Databases database, PlayerInputWrapper playerInput)
        {
            this.playerInput = playerInput;
            this.database = database;
            
            LoadEntity(entity, database);
            
            disableObject.SetActive(true);

            // TODO Add animation
            
            // Do text stuff
            int pointAmount = 1;
            string readText = database.GetTranslatedText("levelup_point_text").Replace("{POINTS}", "<color=#76d3f5>" + pointAmount + "</color>");
            yield return StartCoroutine(battleDialogue.ReadString("system", readText));

            yield return new WaitForSeconds(0.2f);

            // TODO Change points here
            yield return StartCoroutine(battleResultsStatIncrease.ExecuteStatScreen(entity, 1));
            
            yield return null;
        }
        
        private void LoadEntity(Entity.Entity entity, Databases database)
        {
            SpriteUtils.SpriteUtils.ApplySpriteToImage(entitySprite, entity.sprites[0]);
            selectedUI.SelectEntity(entity, database);
            if (entity is Human human)
            {
                levelupEquipment.LoadInformation(human, database);
            }
        }

    }
}