using Game.Level;
using Game.Minimap;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class FloorUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text floorValueText;

        [SerializeField] private MapLoader mapLoader;
        
        private int currentFloorNumber = 0;
        private string currentAreaName = "";

        public void CheckForDifference()
        {
            if (mapLoader.GetCurrentMapData() == null)
            {
                // Update via mapLoader's data
                CheckForDifference(mapLoader.GetCurrentMapName(), mapLoader.GetCurrentFloorNum());
            }
            else
            {
                // Update via MapData's data
                CheckForDifference(mapLoader.GetCurrentMapData().GetMapInfo().mapName, mapLoader.GetCurrentMapData().GetCurrentMainFloor());
            }
        }

        private void CheckForDifference(string areaName, int floorNumber)
        {
            if (!currentAreaName.Equals(areaName) || currentFloorNumber != floorNumber)
            {
                UpdateFloorText(areaName, floorNumber);
            }
        }

        public void UpdateFloorText(string areaName, int floorNumber)
        {
            floorValueText.text = areaName + ", " + floorNumber + "F";
        }
    }
}