using UnityEngine;

namespace Game.Console.Commands
{
    [CreateAssetMenu(menuName = "Commands/Quit", fileName = "Quit Command")]
    public class QuitCommand : ConsoleCommand
    {
        public override bool Process(string[] args, string sceneName, out string completionText)
        {
            completionText = "Closing application.";
            Application.Quit();
            return true;
        }
    }
}