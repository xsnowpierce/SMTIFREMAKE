namespace Game.Interactable
{
    public interface IInteractable
    {
        void Interact();

        bool isInteractable();

        string getInteractableName();
    }
}