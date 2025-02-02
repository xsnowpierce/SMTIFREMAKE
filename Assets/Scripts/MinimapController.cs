using System;
using Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Minimap
{
    public class MinimapController : MonoBehaviour
    {
        [Serializable]
        public struct MinimapData
        {
            public string mapID;
            public Sprite mapImage;
        }

        [SerializeField] private RectTransform tileParent;
        [SerializeField] private RectTransform scaleObject;
        [SerializeField] private int tileSize = 15;
        [SerializeField] private float actualTileDistance = 10;
        [SerializeField] private RectTransform mapParent;
        [SerializeField] private RectTransform compassPlayerRotation;

        [Header("Settings")] 
        [SerializeField] [Range(0f, 100f)] private float mapZoom = 10f;
        
        private MinimapData minimapData;
        private SaveData data;
        private Image currentMapObject;
        
        
        public void LoadMap(string mapID, int floorNumber, Sprite image)
        {
            if(currentMapObject != null)
                Destroy(currentMapObject);

            if (image == null)
            {
                return;
            }
            
            // Create the map
            GameObject mapObject = new GameObject();
            Image mapImage = mapObject.AddComponent<Image>();
            mapImage.name = "Map Image";

            mapImage.rectTransform.SetParent(mapParent);
            mapImage.rectTransform.sizeDelta = new Vector2(image.texture.width, image.texture.height);
            mapImage.sprite = image;
            mapImage.rectTransform.localScale = new Vector3(1, 1, 1);
            mapImage.rectTransform.anchoredPosition3D = new Vector3();
            mapObject.layer = 5;

            // Set position, rotation, and size of object
            mapImage.rectTransform.anchoredPosition = new Vector2((image.texture.width / 2f) - tileSize / 2f, (image.texture.height / 2f) - tileSize / 2f);
            currentMapObject = mapImage;
            
            
            /*
            // Save our last one first
            SaveMapData();
            // Unload minimap
            UnloadMinimap();
            
            if (image == null)
            {
                // No map was given
                Debug.LogError("No minimap was found.");
                return;
            }
            
            MinimapData loadData = data.minimapDatas.FirstOrDefault(map => map.mapID.Equals(mapID + floorNumber));
            minimapData = loadData;
            minimapData.mapImage = image;
            tiles = new GameObject[(int) image.rect.width / tileSize, (int) image.rect.height / tileSize];
            
            if (loadData.mapImage == null || loadData.mapTiles == null)
            {
                // Minimap doesn't exist, set it up
                minimapData.mapID = mapID + floorNumber;
                minimapData.mapImage = image;
                minimapData.mapTiles = new sbyte[(int) image.rect.width / tileSize, (int) image.rect.height / tileSize];
                
                // Add hidden tiles to each position
                for (int i = 0; i < (int) image.rect.width / tileSize; i++)
                {
                    for (int j = 0; j < (int) image.rect.height / tileSize; j++)
                    {
                        AddTile(new Vector2(i, j));
                    }
                }
            }
            else
            {
                // Map exists, load it 
                // Minimap exists, load it
                for (int i = 0; i < loadData.mapTiles.GetLength(0); i++)
                {
                    for (int j = 0; j < loadData.mapTiles.GetLength(1); j++)
                    {
                        if (minimapData.mapTiles[i, j] == 1)
                            continue;
                        else AddTile(new Vector2(i, j));
                    }
                }
            }
            */
        }

        /*
        private void Awake()
        {
            UpdateScale(mapZoom);
            data = FindObjectOfType<SaveData>();
            minimapData = new MinimapData()
            {
                mapImage = null,
                mapID = null
            };
        }

        public void PlayerMovement(Vector3 position)
        {
            DestroyHiddenTile(Mathf.RoundToInt(position.x / actualTileDistance), Mathf.RoundToInt(position.z / actualTileDistance));
        }
        
        public void UpdateCompassRotation(float y)
        {
            compassPlayerRotation.localRotation = Quaternion.Euler(new Vector3(0, 0, -y));
        }
        
        private void SetTilePosition(int x, int y, sbyte tileValue)
        {
            minimapData.mapTiles[x, y] = tileValue;
        }

        private void DestroyHiddenTile(int x, int y)
        {
            // TODO have some sort of check to see if the player is on a staircase
            if (x < 0 || y < 0)
            {
                Debug.LogWarning("Player walked into negative space. Was this intended?");
                return;
            }
            
            if (minimapData.mapTiles[x, y] == 0) 
            { 
                minimapData.mapTiles[x, y] = 1; 
                Destroy(tiles[x, y]);
            }
        }

        private void UnloadMinimap()
        {
            if (tiles != null)
            {
                for (int i = 0; i < tiles.GetLength(0); i++)
                {
                    for (int j = 0; j < tiles.GetLength(1); j++)
                    {
                        if (tiles[i, j] != null)
                        {
                            Destroy(tiles[i, j]);
                        }
                    }
                }
            }
            
            if (currentMapObject != null)
            {
                // Destroy object
                Destroy(currentMapObject.gameObject);
                currentMapObject = null;
            }

            tiles = new GameObject[0, 0];
        }

        public void SaveMapData()
        {
            if (string.IsNullOrEmpty(minimapData.mapID))
            {
                // Don't save, we aren't in a map
                return;
            }

            // In a map, save it!

            // Remove possible duplicates of our map
            data.minimapDatas.RemoveAll(map => map.mapID.Equals(minimapData.mapID));
            // Add the updated one back in
            data.minimapDatas.Add(minimapData);
        }

        public void UpdatePosition(Vector3 position)
        {
            Vector2 playerPositionNormalized =
                new Vector2(position.x / actualTileDistance, position.z / actualTileDistance);

            tileParent.anchoredPosition = new Vector2(-playerPositionNormalized.x * tileSize,
                -playerPositionNormalized.y * tileSize);
            mapParent.anchoredPosition = new Vector2(-playerPositionNormalized.x * tileSize,
                -playerPositionNormalized.y * tileSize);
        }

        public void UpdateScale(float zoom)
        {
            mapZoom = zoom;
            scaleObject.localScale = new Vector3(zoom, zoom, zoom);
        }

        public Vector2 PositionToGrid(Vector3 position)
        {
            Vector2 pos = new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
            if (pos.x != 0)
                pos.x /= actualTileDistance;
            if (pos.y != 0)
                pos.y /= actualTileDistance;

            return pos;
        }

        public void AddTile(Vector2 tilePosition)
        {
            GameObject newIcon = new GameObject();
            Image newImage = newIcon.AddComponent<Image>();
            newImage.name = "Map Tile";
            newImage.color = Color.black;

            newImage.rectTransform.SetParent(tileParent);
            newImage.rectTransform.sizeDelta = new Vector2(tileSize, tileSize);
            newImage.rectTransform.localScale = new Vector3(1, 1, 1);
            newImage.rectTransform.anchoredPosition3D = new Vector3();
            newIcon.layer = 5;

            // Set position, rotation, and size of object
            newImage.rectTransform.anchoredPosition = tilePosition * tileSize;
            tiles[(int) tilePosition.x, (int) tilePosition.y] = newIcon;
        }

        public GameObject GetCopyObject() => scaleObject.gameObject;
        */
    }
    
}