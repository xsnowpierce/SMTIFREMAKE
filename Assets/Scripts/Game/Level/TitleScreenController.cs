using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Level
{
    public class TitleScreenController : MonoBehaviour
    {
        [Header("Game Text Settings")] [SerializeField]
        private TMP_Text[] texts;

        [Header("Items")] 
        [SerializeField] private PlayerInput input;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Animator gateAnimator;
        [SerializeField] private GameObject canvasObject;
        [SerializeField] private Animator volumeAnimator;
        [SerializeField] private Animator cloudsAnimator;
        [SerializeField] private Animator sunAnimator;
        private Animator camAnimator;
        
        [Header("Camera Settings")]
        [SerializeField] private Vector3 cameraPosition;
        [SerializeField] private Vector3 cameraRotation;
        [SerializeField] private float cameraFOV;
        
        private Camera cam;
        private Coroutine animationCutscene;
        private int currentI;

        private void Awake()
        {
            input.actions["AnyButton"].performed += _ => ButtonPress();
            canvasObject.SetActive(true);
            cam = Camera.main;
            camAnimator = cam.GetComponent<Animator>();

            foreach (TMP_Text text in texts)
            {
                text.color = new Color(1, 1, 1, 0);
                text.enabled = true;
            }

            backgroundImage.enabled = true;
            backgroundImage.color = new Color(0, 0, 0, 1f);
        }

        private void Start()
        {
            camAnimator.Play("Camera", 0, 0.0f);
            animationCutscene = StartCoroutine(runtime());
        }

        private void ButtonPress()
        {
            if (animationCutscene == null) return;
            StopCoroutine(animationCutscene);
            StartCoroutine(AfterCutscene());
        }

        private IEnumerator runtime()
        {
            yield return new WaitForSeconds(0.5f);
            //
            gateAnimator.Play("Gate", 0, 0f);
            for (float j = 1f; j >= 0; j -= Time.deltaTime)
            {
                // set color with i as alpha
                backgroundImage.color = new Color(0, 0, 0, j);
                yield return null;
            }

            backgroundImage.color = new Color(0, 0, 0, 0);


            // Wait for 2 seconds
            yield return new WaitForSeconds(2f);
            
            currentI = 0;
            for (int i = 0; i < texts.Length - 2; i++)
            {
                currentI = i;
                backgroundImage.color = new Color(0, 0, 0, 1);
                texts[i].color = new Color(1, 1, 1, 1);
                gateAnimator.StopPlayback();

                yield return new WaitForSeconds(1f);

                // Hide text
                for (float j = 1f; j > 0f; j -= Time.deltaTime)
                {
                    // set color with i as alpha
                    texts[i].color = new Color(1, 1, 1, j);
                    yield return null;
                }

                gateAnimator.Play("Gate", 0);
                backgroundImage.color = new Color(0, 0, 0, 0);
                texts[i].color = new Color(1, 1, 1, 0);
                yield return new WaitForSeconds(1.5f);
            }

            // Last two now
            camAnimator.enabled = false;
            cam.transform.position = cameraPosition;
            cam.transform.rotation = Quaternion.Euler(cameraRotation);
            cam.fieldOfView = cameraFOV;

            backgroundImage.color = new Color(0, 0, 0, 1);
            texts[^2].color = new Color(1, 1, 1, 1);
            gateAnimator.StopPlayback();
            
            yield return new WaitForSeconds(1f);
            
            for (float j = 0f; j < 1f; j += Time.deltaTime)
            {
                // set color with i as alpha
                texts[^1].color = new Color(1, 1, 1, j);
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            // Hide both texts
            for (float j = 1f; j > 0f; j -= Time.deltaTime)
            {
                // set color with i as alpha
                texts[^1].color = new Color(1, 1, 1, j);
                texts[^2].color = new Color(1, 1, 1, j);
                yield return null;
            }
            
            texts[^1].color = new Color(1, 1, 1, 0);
            texts[^2].color = new Color(1, 1, 1, 0);
            
            // Fade out black screen
            for (float j = 1f; j >= 0; j -= Time.deltaTime)
            {
                // set color with i as alpha
                backgroundImage.color = new Color(0, 0, 0, j);
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
            camAnimator.enabled = true;
            camAnimator.Play("CamShake", 0, 0f);
            yield return new WaitForSeconds(0.5f);
            volumeAnimator.Play("Volume", 0, 0f);
            cloudsAnimator.Play("Clouds", 0, 0f);
            sunAnimator.Play("Sun", 0, 0f);
            yield return new WaitForSeconds(0.5f);
            camAnimator.enabled = false;
            // End
            yield return null;
        }

        private IEnumerator AfterCutscene()
        {
            float lastAlpha = backgroundImage.color.a;
            for (float j = lastAlpha; j < 1; j += Time.deltaTime * 2)
            {
                // set color with i as alpha
                backgroundImage.color = new Color(0, 0, 0, j);
                yield return null;
            }
            backgroundImage.color = new Color(0, 0, 0, 1);

            yield return new WaitForSeconds(0.5f);
            
            // Progress to main menu
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        }
    }
}