namespace Gint
{
    public interface ICommandOutput
    {
        CommandState CommandState { get; }
    }

    public enum CommandState
    {
        Success,
        Error
    }
}
