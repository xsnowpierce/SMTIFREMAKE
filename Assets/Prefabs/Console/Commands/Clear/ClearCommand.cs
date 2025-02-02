using UnityEngine;

namespace Game.Console.Commands.Clear
{
    [CreateAssetMenu(fileName = "Clear Command", menuName = "Commands/Clear")]
    public class ClearCommand : ConsoleCommand
    {
        public override bool Process(string[] args, string sceneName, out string completionText)
        {
            DeveloperConsole console = FindObjectOfType<DeveloperConsole>();
            console.GetConsoleText().SetText("");
            completionText = "The console has been cleared.";
            return true;
        }
    }
}