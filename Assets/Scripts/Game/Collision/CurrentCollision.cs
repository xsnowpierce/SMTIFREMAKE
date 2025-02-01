using System.Collections.Generic;
using Game.Level;
using Game.Minimap;
using Game.Movement;
using UnityEngine;

namespace Game.Collision
{
    public class CurrentCollision : MovementCollision
    {
        private Transform parent;
        [SerializeField] private MusicController musicController;
        [SerializeField] private PlayerFlashlight playerFlashlight;
        [SerializeField] private List<MapCollider> colliders;
        
        private void Awake()
        {
            colliders = new List<MapCollider>();
            parent = FindObjectOfType<Movement.Movement>().transform;
        }

        public void ReloadList()
        {
            List<MapCollider> newList = new List<MapCollider>();
            foreach (MapCollider coll in colliders)
            {
                if (coll != null && coll.isActiveAndEnabled)
                    newList.Add(coll);
            }

            colliders = newList;
        }

        public void CheckForFlashlightZone()
        {
            bool useFlashlight = false;
            foreach (MapCollider coll in colliders)
            {
                if (coll is not FlashlightCollider) continue;
                
                Debug.Log("flashlight found");
                useFlashlight = true;
            }
            playerFlashlight.ToggleFlashlight(useFlashlight);
        }
        
        protected override void OnTriggerEnter(Collider other)
        {
            MapCollider otherCollider = other.GetComponent<MapCollider>();
            if (otherCollider == null) return;
            
            if (otherCollider is MusicCollider musicCollider)
            {
                // Change the music
                if (string.IsNullOrEmpty(musicCollider.GetMapID()))
                {
                    // Stop current music
                    musicController.StopSong();
                }
                else
                {
                    // Change to this song
                    musicController.ChangeToSong(musicController.GetCurrentMusicPack().GetMusicFromString(musicCollider.GetMapID()));
                }
            }

            if (otherCollider is FlashlightCollider)
            {
                // Enable flashlight
                playerFlashlight.ToggleFlashlight(true);
            }

            if (otherCollider is EventTile eventTile)
            {
                eventTile.OnTileEntered(parent.rotation.eulerAngles);
            }
            colliders.Add(otherCollider);
            base.OnTriggerEnter(other);
        }

        protected override void OnTriggerExit(Collider other)
        {
            MapCollider otherCollider = other.GetComponent<MapCollider>();
            if (otherCollider == null) return;
            
            if (otherCollider is FlashlightCollider)
            {
                // Disable flashlight
                playerFlashlight.ToggleFlashlight(false);
            }

            if (otherCollider is EventTile eventTile)
            {
                eventTile.OnTileExited();
            }
            colliders.Remove(otherCollider);
            base.OnTriggerEnter(other);
        }
    }
}