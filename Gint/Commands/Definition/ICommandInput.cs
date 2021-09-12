namespace Gint
{
    public interface ICommandInput
    {
        string Variable { get; }
        InputStream Stream { get; }
        int ExecutionId { get; }
        string[] Options { get; }
    }
}
