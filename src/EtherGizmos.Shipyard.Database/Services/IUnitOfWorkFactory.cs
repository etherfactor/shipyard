namespace EtherGizmos.Shipyard.Services;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create(bool useRequestScope = false);
}
