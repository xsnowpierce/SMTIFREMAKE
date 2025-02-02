using System;
using System.Linq;
using Game.Console.Commands;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game.Console
{
    public class DeveloperConsole : MonoBehaviour
    {
        [SerializeField] private ConsoleCommand[] commands;
        [Header("UI")] 
        [SerializeField] private GameObject uiCanvas;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Text consoleText;

        private bool consoleOpened;
        private float lastTimeScale;
        private PlayerInput input;
        private string lastInput;
        private readonly string unknownCommand = "<color=red>Command was unknown. Please try again.</color>";
        static DeveloperConsole instance;
        
        private void Awake()
        {
            //Singleton method
            if (instance == null)
            {
                //First run, set the instance
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                //Instance is not the same as the one we have, destroy old one, and reset to newest one
                Destroy(instance.gameObject);
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            
            input = FindObjectOfType<PlayerInput>();
            input.actions["ConsoleOpen"].performed += _ => ToggleConsole();
            input.actions["ConsoleProcess"].performed += _ => SendCommand(inputField.text);
            input.actions["ConsoleUpArrow"].performed += _ => CastPastInput();
        }

        public void ToggleConsole()
        {
            if (consoleOpened)
            {
                // Resume game
                Time.timeScale = lastTimeScale;
                inputField.text = "";
                uiCanvas.SetActive(false);
                inputField.DeactivateInputField();
                consoleOpened = false;
            }
            else
            {
                // Open console
                if (input == null)
                {
                    input = FindObjectOfType<PlayerInput>();
                    input.actions["ConsoleOpen"].performed += _ => ToggleConsole();
                    input.actions["ConsoleProcess"].performed += _ => SendCommand(inputField.text);
                    input.actions["ConsoleUpArrow"].performed += _ => CastPastInput();
                }
                lastTimeScale = Time.timeScale;
                Time.timeScale = 0;
                uiCanvas.SetActive(true);
                inputField.ActivateInputField();
                inputField.Select();
                consoleOpened = true;
            }
        }

        public void CastPastInput()
        {
            if (!consoleOpened) return;
            
            inputField.text = lastInput;
            inputField.caretPosition = inputField.text.Length + 1;
        }

        public void SendCommand(string inputValue)
        {
            if (!consoleOpened) return;
            
            lastInput = inputValue;
            if (inputValue.Trim() == string.Empty)
            {
                inputField.text = "";
                return;
            }
            string outputText = ProcessCommand(inputValue);
            consoleText.text += "\n" + outputText;
            inputField.text = "";
            inputField.Select();
            inputField.ActivateInputField();
        }
        
        private string ProcessCommand(string inputValue)
        {
            string[] inputSplit = inputValue.Split(' ');
            string commandInput = inputSplit[0];
            string[] args = inputSplit.Skip(1).ToArray();
            string sceneName = SceneManager.GetActiveScene().name;
            
            // Search every command in our command array
            foreach (IConsoleCommand command in commands)
            {
                // Search each alias the command has
                foreach (string alias in command.CommandWord)
                {
                    // Continue if the command doesn't equal the alias
                    if (!commandInput.Equals(alias, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    
                    // Process command
                    command.Process(args, sceneName, out string text);
                    return text;
                }
            }
            // No command was found
            return unknownCommand;
        }

        public TMP_Text GetConsoleText() => consoleText;
        public bool GetConsoleOpened() => consoleOpened;
    }
}