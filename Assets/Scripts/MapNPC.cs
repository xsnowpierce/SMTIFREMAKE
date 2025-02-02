using System;
using Dialogue;
using UnityEngine;

namespace Game.WorldMap
{
    public class MapNPC : WorldMapInteractable
    {
        [Serializable]
        public struct WorldMapDialogue
        {
            public TextAsset inkAsset;
            public string requiredFlag;
        }
        
        [SerializeField] private Sprite hasDialogue, noDialogue;
        [SerializeField] private Transform billboardObject;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private WorldMapDialogue[] dialogue;
        private Camera camMain;

        private void Awake()
        {
            camMain = Camera.main;
            // Check if the player has new dialogue
        }

        private void Update()
        {
            // Update look of spriteRenderer
            Vector3 camPos = camMain.transform.position;
            camPos.x = transform.position.x;
            billboardObject.LookAt(camPos);
        }

        public void SetReadState(bool hasRead)
        {
            if (hasRead)
            {
                spriteRenderer.sprite = noDialogue;
            }
            else spriteRenderer.sprite = hasDialogue;
        }

        public WorldMapDialogue[] GetMapDialogues() => dialogue;
    }
}