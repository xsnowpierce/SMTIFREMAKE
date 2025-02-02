using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Splash
{
    public class SplashController : MonoBehaviour
    {
        [Header("Logos")] [SerializeField] private Image logoHolder;
        [SerializeField] private Sprite[] logosInOrder;
        [SerializeField] private TMP_Text text;
        [SerializeField] private TMP_Text disclaimerText;
        [SerializeField] private Vector2[] logoSizes;
        [SerializeField] private int atlusLogoNumber;

        [Header("Settings")] [SerializeField] private PlayerInput input;
        [SerializeField] private float startDelay;
        [SerializeField] private float fadeTime;
        [SerializeField] private float skipTime;
        [SerializeField] private float holdDelay;
        [SerializeField] private float delayBetweenLogos;
        [SerializeField] private float waitToLoad;
        [SerializeField] private string loadSceneName;

        private bool pressedButton = false;

        private void Awake()
        {
            logoHolder.color = new Color(1, 1, 1, 0);
            text.color = new Color(0, 0, 0, 0);
        }

        private void Update()
        {
            InputSystem.onEvent +=
                (eventPtr, device) =>
                {
                    if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
                        return;
                    var controls = device.allControls;
                    var buttonPressPoint = InputSystem.settings.defaultButtonPressPoint;
                    for (var i = 0; i < controls.Count; ++i)
                    {
                        var control = controls[i] as ButtonControl;
                        if (control == null || control.synthetic || control.noisy)
                            continue;
                        if (control.ReadValueFromEvent(eventPtr, out var value) && value >= buttonPressPoint)
                        {
                            pressedButton = true;
                            break;
                        }
                    }
                };
        }

        private void Start()
        {
            disclaimerText.color = new Color(1, 1, 1, 0);
            if (logosInOrder.Length != 0)
                StartCoroutine(Runtime());
            else LoadNextScene();
        }


        private void PressedButton()
        {
            pressedButton = true;
        }

        private IEnumerator Runtime()
        {
            yield return new WaitForSeconds(startDelay);

            for (int i = 0; i < logosInOrder.Length; i++)
            {
                pressedButton = false;
                logoHolder.sprite = logosInOrder[i];
                logoHolder.rectTransform.sizeDelta = logoSizes[i];

                for (float j = 0; j < fadeTime; j += Time.deltaTime)
                {
                    // set color with i as alpha
                    logoHolder.color = new Color(1, 1, 1, j);
                    if (i == atlusLogoNumber)
                    {
                        text.color = new Color(1, 1, 1, j);
                    }

                    if (pressedButton) j = fadeTime;
                    yield return null;
                }

                for (float j = holdDelay; j > 0; j -= Time.deltaTime)
                {
                    if (pressedButton) j = 0;
                    yield return null;
                }

                for (float j = fadeTime; j >= 0; j -= Time.deltaTime)
                {
                    // set color with i as alpha
                    logoHolder.color = new Color(1, 1, 1, j);
                    if (i == atlusLogoNumber)
                    {
                        text.color = new Color(1, 1, 1, j);
                    }

                    if (pressedButton)
                    {
                        j = 0;
                    }
                    yield return null;
                }

                while (pressedButton)
                {
                    for (float j = skipTime; j >= 0; j -= Time.deltaTime)
                    {
                        // set color with i as alpha
                        logoHolder.color = new Color(1, 1, 1, j);
                        if (i == atlusLogoNumber)
                        {
                            text.color = new Color(1, 1, 1, j);
                        }

                        yield return null;
                    }

                    logoHolder.color = new Color(1, 1, 1, 0);
                    text.color = new Color(1, 1, 1, 0);
                    pressedButton = false;
                }
                
                logoHolder.color = new Color(1, 1, 1, 0);
                text.color = new Color(1, 1, 1, 0);
                yield return new WaitForSeconds(delayBetweenLogos);
            }

            yield return new WaitForSeconds(0.5f);

            // Show disclaimer
            for (float j = 0; j < fadeTime; j += Time.deltaTime)
            {
                // set color
                disclaimerText.color = new Color(1, 1, 1, j);
                yield return null;
            }

            for (float j = holdDelay; j > 0; j -= Time.deltaTime)
            {
                yield return null;
            }

            for (float j = fadeTime; j >= 0; j -= Time.deltaTime)
            {
                // set color with i as alpha
                disclaimerText.color = new Color(1, 1, 1, j);
                yield return null;
            }

            disclaimerText.color = new Color(1, 1, 1, 0);

            yield return new WaitForSeconds(waitToLoad);

            // Load next scene
            LoadNextScene();
        }

        private void LoadNextScene()
        {
            SceneManager.LoadScene(loadSceneName, LoadSceneMode.Single);
        }
    }
}