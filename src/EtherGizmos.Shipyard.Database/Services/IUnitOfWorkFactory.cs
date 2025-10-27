namespace EtherGizmos.Shipyard.Database.Services;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create(bool useRequestScope = false);
}
