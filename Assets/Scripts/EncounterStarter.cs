using System.Collections;
using System.Collections.Generic;
using Battle;
using Data;
using Game.Collision;
using Game.Level;
using Game.Movement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Battle
{
    public class EncounterStarter : MonoBehaviour
    {
        [SerializeField] private MoonPhaseSystem moonPhaseSystem;

        private SaveData data;
        
        private void Awake()
        {
            data = FindObjectOfType<SaveData>();
        }
        
        public void EncounterBattle(List<BattleArea> currentBattleAreas)
        {
            StartCoroutine(BattleEncounter(currentBattleAreas));
        }

        private IEnumerator BattleEncounter(List<BattleArea> currentBattleAreas)
        {
            // Preserve to the battle scene
            DontDestroyOnLoad(gameObject);
            
            // Pick list of enemies to fight
            if (currentBattleAreas.Count == 0)
            {
                Debug.LogError("Tried to start a battle with no enemies in the BattleArea.");
                yield break;
            }

            int area = Random.Range(0, currentBattleAreas.Count);
            if (currentBattleAreas[area].GetEnemies == null)
            {
                Debug.LogError("Tried to start a battle with a BattleArea that has no enemies.");
                yield break;
            }
            
            // TODO Take encounter rate into account
            Debug.LogWarning("The encounter rate isn't taken into consideration yet.");
            int enemy = Random.Range(0, currentBattleAreas[area].GetEnemies.Length);
            BattleController.Enemy[] enemies = currentBattleAreas[area].GetEnemies[enemy].demons;
            
            // TODO Do transition animation
            
            // Stop music 
            FindObjectOfType<MusicController>().StopSong();
                
                // Unload current map
            FindObjectOfType<MapLoader>().HideLevel();
            
            // Disable player and UI
            FindObjectOfType<PlayerMovement>().gameObject.SetActive(false);
            FindObjectOfType<UIManager>().gameObject.SetActive(false);
            // Load battle scene
            AsyncOperation loading = SceneManager.LoadSceneAsync("Battle", LoadSceneMode.Additive);
            
            while (!loading.isDone)
            {
                yield return null;
            }
            
            // Set battle as active scene
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Battle"));
            
            // Give enemies to the battle controller
            BattleController battleController = FindObjectOfType<BattleController>();
            battleController.LoadBattle(enemies, BattleType.RandomEncounter);

            // Destroy object
            Destroy(gameObject);
        }
    }
}