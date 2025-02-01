using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.WorldMap
{
    public class MapMarkerCollision : MonoBehaviour
    {
        [SerializeField] private TMP_Text textBox;
        [SerializeField] private string townName;
        private List<WorldMapMarker> mapMarkers;

        private void Awake()
        {
            mapMarkers = new List<WorldMapMarker>();
        }

        private void OnTriggerEnter(Collider other)
        {
            WorldMapMarker marker = other.gameObject.GetComponent<WorldMapMarker>();
            if (marker != null)
            {
                mapMarkers.Add(marker);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            WorldMapMarker marker = other.gameObject.GetComponent<WorldMapMarker>();
            if (marker != null)
            {
                mapMarkers.Remove(marker);
            }
        }

        private void Update()
        {
            if (mapMarkers.Count == 0)
            {
                textBox.text = townName;
            }
            else
            {
                textBox.text = mapMarkers[0].GetMarkerKey();
            }
        }
    }
}