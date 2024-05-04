namespace Marketing_system.DA.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> Save();
    }
}
