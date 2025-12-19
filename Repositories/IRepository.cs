namespace API.Repositories;

public interface IRepository<T> : IRepositoryBase<T, string> where T : class
{
}
