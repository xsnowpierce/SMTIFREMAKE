using System;
using Game.Input;
using Game.Minimap;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.UI
{
    public class LargeMapController : MonoBehaviour
    {
        [SerializeField] private PlayerInputWrapper input;
        private Camera cam;
        private MinimapController minimapController;
        [SerializeField] private RectTransform mapParent;
        [SerializeField] private GameObject disableObject;
        [SerializeField] private Vector2 maxScrollDimensions;
        [SerializeField] private float scrollSensitivity = 1f;
        private bool mapOpened = false;
        private bool canOpenMap = true;
        private RectTransform fullMapObject;
        private float lastTimeScale;
        
        private void Awake()
        {
            cam = Camera.main;
            lastTimeScale = Time.timeScale;
            minimapController = FindObjectOfType<MinimapController>();
            input.GetPlayerInputScript().actions["FullMap"].performed += _ => ToggleMap();
            SetMapOpened(false);
        }

        private void ToggleMap()
        {
            if (Time.timeScale == 0f && !mapOpened) return;
            
            if(canOpenMap)
                SetMapOpened(!mapOpened);
        }

        private void Update()
        {
            if (mapOpened)
            {
                ProcessInput();
            }
        }

        private void ProcessInput()
        {
            Vector2 movement = input.GetFieldMovement();
            if (movement != Vector2.zero)
            {
                Movement(movement);
            }
        }

        private void Movement(Vector2 movement)
        {
            Vector2 pos = fullMapObject.anchoredPosition;
            fullMapObject.anchoredPosition = new Vector2(pos.x + movement.x * (scrollSensitivity * Time.deltaTime),
                pos.y + movement.y * (scrollSensitivity * Time.deltaTime));
        }
        
        private void SetMapOpened(bool value)
        {
            mapOpened = value;

            if (value)
            {
                fullMapObject.anchorMin = new Vector2(0.5f, 0.5f);
                fullMapObject.anchorMax = new Vector2(0.5f, 0.5f);
                disableObject.SetActive(true);

                lastTimeScale = Time.timeScale;
                Time.timeScale = 0f;
            }
            else
            {
                if(fullMapObject != null)
                    Destroy(fullMapObject.gameObject);
                disableObject.SetActive(false);

                Time.timeScale = lastTimeScale;
            }
        }

        public void SetCanOpenMap(bool value)
        {
            if(!value)
                SetMapOpened(false);

            canOpenMap = value;
        }
    }
}