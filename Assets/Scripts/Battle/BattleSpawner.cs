using System;
using System.Collections.Generic;
using Battle;
using Entity;
using SpriteUtils;
using UnityEngine;

namespace Game.Battle
{
    public class BattleSpawner : MonoBehaviour
    {
        [Header("Sprite Stuff")]
        [SerializeField] private GameObject spritePrefab;
        [SerializeField] private Material enemySpriteMaterial;
        [SerializeField] private int spriteSizeDiv = 185;
        [SerializeField] [Range(0f, 5f)] private float spriteDistancePercentage;

        [Header("Settings")]
        [SerializeField] private float frontRowZ;
        [SerializeField] private float backRowZ;
        [SerializeField] private Transform frontRowParent;
        [SerializeField] private Transform backRowParent;

        private Transform[] enemyTransforms;
        private BattleSpriteLoader[] enemySprites;
        private BattleController controller;

        public void LoadEnemies(BattleController.Enemy[] enemiesList, BattleController battleController)
        {
            enemyTransforms = new Transform[enemiesList.Length];
            enemySprites = new BattleSpriteLoader[enemiesList.Length];
            
            controller = battleController;
            // Spawn the enemies!
            SpawnEnemies(enemiesList);
        }

        private void SpawnEnemies(BattleController.Enemy[] enemy)
        {
            Vector3 lastSpawnPos = new Vector3();
            Renderer lastSpawnRenderer = null;
            bool dealtWithBackRow = false;
            for (int i = 0; i < enemy.Length; i++)
            {
                Entity.Entity ent = enemy[i].demonStats;
                
                if (ent != null)
                {
                    if (!controller.IsInFrontRow(i) && !dealtWithBackRow)
                    {
                        lastSpawnPos = new Vector3();
                        lastSpawnRenderer = null;
                        dealtWithBackRow = true;
                    }
                    
                    Vector3 spawnPosition = lastSpawnPos;
                    spawnPosition.z = controller.IsInFrontRow(i) ? frontRowZ : backRowZ;
                    Transform parent = controller.IsInFrontRow(i) ? frontRowParent : backRowParent;
                    
                    spawnPosition.x = lastSpawnPos.x;
                    // Take the spawnPosition and change it to make sure we have enough spacing from the last demon placed
                    // Get the bounds of the sprite and then multiply that by spriteDistancePercentage
                    // and then add that distance to the spawnPosition
                    if (lastSpawnRenderer != null)
                    {
                        float size = lastSpawnRenderer.bounds.size.x;
                        size *= spriteDistancePercentage;
                        spawnPosition.x = lastSpawnPos.x += size;
                    }
                    
                    GameObject newEntity = Instantiate(spritePrefab, spawnPosition, Quaternion.identity, parent);
                    newEntity.name = enemy[i].demonStats.entityName;
                    enemy[i].arrayID = i;
                    enemy[i].spriteLoader = newEntity.GetComponent<BattleSpriteLoader>();
                    enemy[i].spriteLoader.LoadSprite(enemy[i].demonStats, spriteSizeDiv);
                    enemy[i].spriteLoader.SetBackRow(i >= controller.GetMaxEnemiesInRow());
                    
                    // Add to our array
                    enemyTransforms[i] = newEntity.transform;
                    enemySprites[i] = newEntity.GetComponentInChildren<BattleSpriteLoader>();
                    
                    // Apply to battle controller as well
                    controller.SetEnemyPosition(i, newEntity.transform, enemy[i].demonStats, newEntity.GetComponentInChildren<MeshRenderer>().gameObject);
                    
                    lastSpawnPos = newEntity.transform.position;
                    lastSpawnRenderer = newEntity.GetComponentInChildren<Renderer>();
                }
            }
            
            // Make the group of enemies centered on screen!
            // frontRowParent, backRowParent
            // Get the bounds of all of the objects from leftmost point to rightmost point
            // Then divide that by two to get the center of all of them
            // And then change the position of the parent by this value
            
            for (int i = 0; i < 2; i++)
            {
                Transform currentTransform = i == 0 ? frontRowParent : backRowParent;
                
                Bounds bounds = new Bounds(currentTransform.position, currentTransform.position);
                Renderer[] renderers = currentTransform.GetComponentsInChildren<Renderer>();
                foreach (Renderer rndr in renderers)
                {
                    bounds.Encapsulate(rndr.bounds);
                }

                Vector3 localCenter = bounds.center - transform.position;
                bounds.center = localCenter;
            
                // Change the position of the parent to offset this value
                Vector3 newPosition = currentTransform.position;
                newPosition.x -= bounds.center.x;

                currentTransform.position = newPosition;
            }
            
        }

        public List<Entity.Entity> ProcessTurnOrder(Entity.Entity[] entities)
        {
            List<Entity.Entity> order = new List<Entity.Entity>();
            // If the MC is using a command or item, they will always go first.
            // After that, it is completely up to agility stat on who does their turn.
            // It does not matter which team the entity is on, it is only their speed stat that matters.

            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] is Human character)
                {
                    // Check if the player is using a command or item.
                }
            }
            return order;
        }

        public Transform GetTransform(int id)
        {
            return enemyTransforms[id];
        }

        public BattleSpriteLoader[] GetEnemyRenderers() => enemySprites;
        
    }
    
}