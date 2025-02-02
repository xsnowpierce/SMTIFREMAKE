using Tayx.Graphy;
using UnityEngine;

namespace Game.Console.Commands
{
    [CreateAssetMenu(menuName = "Commands/Performance", fileName = "Performance Command")]
    public class PerformanceCommand : ConsoleCommand
    {
        [SerializeField] private GameObject graphObject;

        public override bool Process(string[] args, string sceneName, out string completionText)
        {
            if (FindObjectOfType<GraphyManager>() == null)
            {
                Instantiate(graphObject);
                completionText = "Stats overlay enabled.";
                return true;
            }

            // End fps tracker
            GraphyManager tracker = FindObjectOfType<GraphyManager>();
            Destroy(tracker.gameObject);
            completionText = "Stats overlay disabled.";
            return true;
        }
    }
}