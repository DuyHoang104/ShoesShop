namespace ShoesShop.Infrastructure.Data.UOW;

public interface IUnitOfWorkManager
{
    IUnitOfWork Create();
    Task<IUnitOfWork> RentAsync(CancellationToken cancellationToken = default);
}