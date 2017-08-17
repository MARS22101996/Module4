namespace CachePower.WEB.Interfaces
{
    public interface IJob
    {
        string JobName { get; }

        void Execute();
    }
}