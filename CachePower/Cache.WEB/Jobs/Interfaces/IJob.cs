using Cache.DAL.Enums;

namespace Cache.WEB.Interfaces
{
    public interface IJob
    {
        JobType Name { get; }

        void Run();
    }
}