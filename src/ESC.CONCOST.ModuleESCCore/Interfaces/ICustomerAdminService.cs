using ESC.CONCOST.Abstract;
using RestEase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleESCCore.Interfaces;

[BasePath("api/CustomerAdmin")]
public interface ICustomerAdminService : IServiceBase
{
    [Get("GetCustomers")]
    Task<List<Customer>> GetCustomersAsync();

    [Post("Approve/{guid}")]
    Task<Result> ApproveAsync([Path] Guid guid);

    [Post("Reject/{guid}")]
    Task<Result> RejectAsync([Path] Guid guid, [Body] string reason);

    [Post("SetPaid/{guid}")]
    Task<Result> SetPaidAsync([Path] Guid guid, [Body] bool isPaid);

    [Post("SetMembership/{guid}")]
    Task<Result> SetMembershipAsync([Path] Guid guid, [Body] string membershipType);
    [Get("GetCustomer/{guid}")]
    Task<ResultOf<Customer>> GetCustomerAsync([Path] Guid guid);
}