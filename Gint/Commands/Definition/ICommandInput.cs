namespace Gint
{
    public interface ICommandInput
    {
        string Variable { get; }
        string Stream { get; }
        int ExecutionId { get; }
    }
}
