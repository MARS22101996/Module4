using Cache.DAL.Enums;

namespace Cache.WEB.Jobs.Interfaces
{
    public interface IJob
    {
        JobType Name { get; }

        void Run();
    }
}