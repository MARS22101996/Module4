namespace CachePower.WEB.Interfaces
{
    public interface IJob
    {
        string Name { get; }

        void Run();
    }
}