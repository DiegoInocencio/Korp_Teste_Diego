namespace Estoque.Domain.Interfaces;

public interface IRepositoryTransaction : IAsyncDisposable
{
    Task CommitAsync();
    Task RollbackAsync();
}
