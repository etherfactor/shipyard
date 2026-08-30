namespace EtherGizmos.Shipyard;

public static class AppConstants
{
    public static class Users
    {
        public static readonly Guid WorkerSystemId = new("4be7c813-ba02-4aef-ae67-d2dfec8b28b6");
    }

    public static class Groups
    {
        public static readonly Guid DefaultSystemId = new("86c51dd9-c62d-49a5-9fa1-87dbd5a95cb5");
    }

    public static class Roles
    {
        public static readonly Guid SystemOwnerSystemId = new("1706f63d-9bc5-4251-bf61-a50d5c705e08");
        public static readonly Guid CarrierManagerSystemId = new("a1008dc3-510a-47cf-a6fa-92cf13c9574c");
        public static readonly Guid UserManagerSystemId = new("73b5274e-d972-4669-9a2f-8c1cfe0318dd");
        public static readonly Guid GroupOwnerSystemId = new("1a72edd6-cc8f-4cb6-b7c6-39364dc73d6f");
        public static readonly Guid MemberSystemId = new("24f66204-1747-4e44-868c-b9fcd656a772");
        public static readonly Guid ViewerSystemId = new("c1f04075-4c1a-493d-a71b-0177c4d8def1");
    }

    public static class Applications
    {
        public static readonly Guid WebUIClientId = new("1c9cc927-68fe-4376-8d3f-b71ef15289b6");
        public static readonly Guid WorkerClientId = new("608b511a-737b-4e9a-90f5-64fb86b8469f");
    }

    public static class Scopes
    {
        public const string EntireApp = "app"; //Legacy
        public const string CarrierRead = "carrier.read";
        public const string CarrierWrite = "carrier.write";
        public const string CarrierDelete = "carrier.delete";
        public const string CarrierExecutionRead = "carrier-execution.read";
        public const string GroupRead = "group.read";
        public const string GroupWrite = "group.write";
        public const string GroupDelete = "group.delete";
        public const string PackageRead = "package.read";
        public const string PackageWrite = "package.write";
        public const string PackageDelete = "package.delete";
        public const string RoleRead = "role.read";
        public const string TrackingUpdateRead = "tracking-update.read";
        public const string UserRead = "user.read";
        public const string UserWrite = "user.write";
        public const string UserDelete = "user.delete";
    }
}
