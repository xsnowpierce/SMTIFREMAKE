using System.Collections;
using System.Collections.Generic;
using Game.Battle;
using Game.Collision;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Encounter
{
    public class EnemyEncounterController : MonoBehaviour
    {
        private UIManager uiManager;
        private int currentEnemyIntensity;
        private List<BattleArea> battleAreas;
        
        [Header("Settings")]
        [SerializeField] private int maxEnemyIntensity;
        [SerializeField] [Range (0f, 1f)] private float percentageChanceToIncrease;

        [Header("UI Elements")] 
        [SerializeField] private Image encounterImage;
        [SerializeField] private Color[] stageColors;
        [SerializeField] private Vector2 stageFadeBounds;
        [SerializeField] private float[] stageFadeSpeeds;

        private Coroutine fadeAnimation;

        private void Awake()
        {
            battleAreas = new List<BattleArea>();
        }
        
        public void AddBattleArea(BattleArea area)
        {
            if(!battleAreas.Contains(area))
                battleAreas.Add(area);

            UpdateImage();
        }

        public void RemoveBattleArea(BattleArea area)
        {
            if (battleAreas.Contains(area))
                battleAreas.Remove(area);

            if (battleAreas.Count == 0)
            {
                // Reset counter
                StopEncountering();
            }
        }
        
        public void OnPlayerMove()
        {
            if (battleAreas.Count == 0) return;
            
            float rdm = Random.Range(0f, 1f);
            if (!(rdm <= percentageChanceToIncrease)) return;
            if (currentEnemyIntensity == maxEnemyIntensity)
                // encounter enemy!
                EncounterEnemy(battleAreas);
            else
                IncreaseIntensity();
        }

        private void IncreaseIntensity()
        {
            encounterImage.enabled = true;
            currentEnemyIntensity++;
            UpdateImage();
        }

        private void StopEncountering()
        {
            currentEnemyIntensity = 0;
            if(fadeAnimation != null) StopCoroutine(fadeAnimation);
            encounterImage.enabled = false;
        }

        private void UpdateImage()
        {
            if (currentEnemyIntensity >= 0)
            {
                encounterImage.enabled = true;
                if (fadeAnimation == null) fadeAnimation = StartCoroutine(AnimateIcon());
            }
        }

        public void EncounterEnemy(List<BattleArea> battleAreas)
        {
            StopEncountering();
            
            // Create EncounterStarter instance and start the battle
            GameObject encounterStarter = new GameObject();
            encounterStarter.name = "Encounter Starter";
            EncounterStarter starter = encounterStarter.AddComponent<EncounterStarter>();
            starter.EncounterBattle(battleAreas);
        }

        private IEnumerator AnimateIcon()
        {
            while (true)
            {
                // Fade out
                for (float i = stageFadeBounds.y;
                    i > stageFadeBounds.x;
                    i -= stageFadeSpeeds[currentEnemyIntensity] * Time.deltaTime)
                {
                    Color color = stageColors[currentEnemyIntensity];
                    color.a = i / 255f;
                    encounterImage.color = color;
                    yield return null;
                }
                // Fade in
                for (float i = stageFadeBounds.x;
                    i < stageFadeBounds.y;
                    i += stageFadeSpeeds[currentEnemyIntensity] * Time.deltaTime)
                {
                    Color color = stageColors[currentEnemyIntensity];
                    color.a = i / 255f;
                    encounterImage.color = color;
                    yield return null;
                }
                yield return null;
            }
        }
        public int GetCurrentEnemyIntensity() => currentEnemyIntensity;
    }
}