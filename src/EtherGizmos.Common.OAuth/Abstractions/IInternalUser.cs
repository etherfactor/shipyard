namespace EtherGizmos.Common.Abstractions;

public interface IInternalUser : IUser
{
    string PasswordHash { get; set; }
}
