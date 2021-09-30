namespace Gint
{
    public interface ICommandInput
    {
        string Variable { get; }
        int ExecutionId { get; }
        string[] Options { get; }
        CommandScope Scope { get; }
    }
}
