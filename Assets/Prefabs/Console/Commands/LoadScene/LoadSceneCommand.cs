using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Console.Commands.Clear
{
    [CreateAssetMenu(fileName = "LoadScene Command", menuName = "Commands/LoadScene")]
    public class LoadSceneCommand : ConsoleCommand
    {
        private string helpText = "Please specify a scene name. Possible scene names are: WorldMap, Dungeon, Battle";
        
        public override bool Process(string[] args, string sceneName, out string completionText)
        {
            if (args.Length == 0)
            {
                completionText = helpText;
                return true;
            }
            else if (args.Length == 1)
            {
                if (string.IsNullOrEmpty(args[0]) || string.IsNullOrWhiteSpace(args[0]))
                {
                    completionText = helpText;
                    return true;
                }
                if (args[0].ToLower().Equals(sceneName.ToLower()))
                {
                    completionText = "You are already present in this scene.";
                    return true;
                }
                else
                {
                    SceneManager.LoadScene(args[0], LoadSceneMode.Single);
                    completionText = "Loading scene " + args[0];
                    Time.timeScale = 1;
                    return true;
                }
            }
            else if(args.Length > 1)
            {
                completionText = "The console has been cleared.";
                return true;
            }
            
            completionText = errorMessage;
            return false;
        }
    }
}