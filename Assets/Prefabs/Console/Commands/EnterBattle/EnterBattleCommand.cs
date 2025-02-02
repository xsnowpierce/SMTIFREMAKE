using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Console.Commands.EnterBattle
{
    [CreateAssetMenu(fileName = "EnterBattle Command", menuName = "Commands/EnterBattle")]
    public class EnterBattleCommand : ConsoleCommand
    {
        
        public override bool Process(string[] args, string sceneName, out string completionText)
        {
            SceneManager.LoadScene("Battle", LoadSceneMode.Single);
            completionText = "Entering battle.";
            return true;
        }
    }
}