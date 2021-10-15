namespace Gint
{
    public interface ICommandInput
    {
        string Variable { get; }
        bool HasVariable { get; }
        int ExecutionId { get; }
        string[] Options { get; }
        CommandScope Scope { get; }
    }
}
