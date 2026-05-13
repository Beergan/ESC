namespace ESC.CONCOST.Abstract;

public class TenantInfo
{
    public string Host { get; set; }
}

public interface ITenantSource
{
    TenantInfo[] ListTenants();
}