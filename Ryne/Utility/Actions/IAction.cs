namespace Ryne.Utility.Actions
{
    public interface IAction
    {
        bool Execute();

        bool Undo();
    }
}
