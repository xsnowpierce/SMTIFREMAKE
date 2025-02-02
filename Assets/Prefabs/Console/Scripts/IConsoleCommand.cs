namespace Game.Console.Commands
{
    public interface IConsoleCommand
    {
        abstract string[] CommandWord { get; }
        bool Process(string[] args, string sceneName, out string completionText);
    }
}