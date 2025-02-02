using Dialogue;
using UnityEngine;

namespace Game.Level
{
    public class DoorRoom : MapDoor
    {
        [SerializeField] private RoomSettings roomData;
        [SerializeField] private PossibleDialogue[] dialogues;
        [SerializeField] private bool useNormalCameraSettings;
        [SerializeField] private bool requireBorders;
        
        public RoomSettings GetRoomData() => roomData;
        public bool GetRequireBorders() => requireBorders;
        public PossibleDialogue[] GetDialogues() => dialogues;
        public bool GetNormalCameraSettingsOverride() => useNormalCameraSettings;
    }
}