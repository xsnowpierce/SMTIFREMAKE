using UnityEngine;

namespace Game.Console.Commands
{
    public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
    {
        [SerializeField] protected string[] commandWord;
        [SerializeField] [TextArea] protected string commandDescription;
        [SerializeField] [TextArea] protected string commandUsage;
        protected string errorMessage = "<color=red>An error has occurred.</color>";

        public string[] CommandWord => commandWord;

        public abstract bool Process(string[] args, string sceneName, out string completionText);
    }
}