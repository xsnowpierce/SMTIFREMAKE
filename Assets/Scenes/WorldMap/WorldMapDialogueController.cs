using Dialogue;
using UnityEngine;

namespace Game.WorldMap
{
    public class WorldMapDialogueController : DialogueController
    {
        public void LoadMapDialogue(MapNPC.WorldMapDialogue[] mapDialogue)
        {
            if (dialogueOpened) return;

            if (mapDialogue.Length == 0) return;
            
            currentStory = null;
            MapNPC.WorldMapDialogue? worldMapDialogue = null;
            
            for (int i = mapDialogue.Length; i > 0; i--)
            {
                if (saveData.playerFlags.Contains(mapDialogue[i - 1].requiredFlag) || string.IsNullOrEmpty(mapDialogue[i - 1].requiredFlag))
                {
                    // Use this one
                    worldMapDialogue = mapDialogue[i - 1];
                    break;
                }
            }

            if (worldMapDialogue != null)
            {
                if (worldMapDialogue.Value.inkAsset == null)
                {
                    Debug.LogError("Dialogue was found, but the file was null.");
                    return;
                }
                
                // It was fine, load it
                LoadDialogue(worldMapDialogue.Value.inkAsset);
                OpenDialogue();
            }
            else
            {
                Debug.LogError("Player chatted to NPC without any possible dialogue.");
                return;
            }
        }
    }
}