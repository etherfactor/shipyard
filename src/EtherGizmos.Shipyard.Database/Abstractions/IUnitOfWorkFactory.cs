namespace EtherGizmos.Shipyard.Abstractions;

public interface IUnitOfWorkFactory
{
    IUnitOfWork Create(bool useRequestScope = false);
}
