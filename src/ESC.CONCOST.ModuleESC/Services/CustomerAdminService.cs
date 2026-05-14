using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleESCCore;
using ESC.CONCOST.ModuleESCCore.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleESC.Services;

public class CustomerAdminService : MyServiceBase, ICustomerAdminService
{
    private readonly IWebHostEnvironment _hostingEnv;
    private readonly ILogger<CustomerAdminService> _log;
    private readonly string _enterpriseCode;


    public CustomerAdminService(IMyContext ctx, ILogger<CustomerAdminService> logger, IWebHostEnvironment env) : base(ctx)
    {
        _hostingEnv = env;
        _log = logger;
        _enterpriseCode = _ctx.EnterpriseCode;
    }

    public async Task<List<Customer>> GetCustomersAsync()
    {
        using var db = _ctx.ConnectDb();

        return await db.Repo<Customer>()
            .Query()
            .OrderByDescending(x => x.RequestDate)
            .ToListAsync();
    }

    public async Task<Result> ApproveAsync(Guid guid)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            var customer = await db.Repo<Customer>()
                .GetOneEdit(x => x.Guid == guid);

            if (customer == null)
            {
                return new Result
                {
                    Success = false,
                    Message = _ctx.Text["고객을 찾을 수 없습니다.|Customer was not found."]
                };
            }

            customer.ApprovalStatus = BasicCodes.CustomerApprovalStatus.Approved;
            customer.RejectReason = string.Empty;
            customer.DateModified = DateTime.UtcNow;

            await db.Repo<Customer>().Update(customer);

            return new Result
            {
                Success = true,
                Message = _ctx.Text["승인되었습니다.|Approved successfully."]
            };
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error approving customer. Guid={Guid}", guid);

            return new Result
            {
                Success = false,
                Message = _ctx.Text["승인 중 오류가 발생했습니다.|An error occurred while approving."]
            };
        }
    }

    public async Task<Result> RejectAsync(Guid guid, string reason)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            var customer = await db.Repo<Customer>()
                .GetOneEdit(x => x.Guid == guid);

            if (customer == null)
            {
                return new Result
                {
                    Success = false,
                    Message = _ctx.Text["고객을 찾을 수 없습니다.|Customer was not found."]
                };
            }

            customer.ApprovalStatus = BasicCodes.CustomerApprovalStatus.Rejected;
            customer.RejectReason = reason ?? string.Empty;
            customer.DateModified = DateTime.UtcNow;

            await db.Repo<Customer>().Update(customer);

            return new Result
            {
                Success = true,
                Message = _ctx.Text["거절되었습니다.|Rejected successfully."]
            };
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error rejecting customer. Guid={Guid}", guid);

            return new Result
            {
                Success = false,
                Message = _ctx.Text["거절 중 오류가 발생했습니다.|An error occurred while rejecting."]
            };
        }
    }

    public async Task<Result> SetPaidAsync(Guid guid, bool isPaid)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            var customer = await db.Repo<Customer>()
                .GetOneEdit(x => x.Guid == guid);

            if (customer == null)
            {
                return new Result
                {
                    Success = false,
                    Message = _ctx.Text["고객을 찾을 수 없습니다.|Customer was not found."]
                };
            }

            customer.IsPaid = isPaid;
            customer.DateModified = DateTime.UtcNow;

            await db.Repo<Customer>().Update(customer);

            return new Result
            {
                Success = true,
                Message = _ctx.Text["결제 상태가 변경되었습니다.|Payment status has been updated."]
            };
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error setting paid status. Guid={Guid}", guid);

            return new Result
            {
                Success = false,
                Message = _ctx.Text["결제 상태 변경 중 오류가 발생했습니다.|An error occurred while updating payment status."]
            };
        }
    }

    public async Task<Result> SetMembershipAsync(Guid guid, string membershipType)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            if (!BasicCodes.MembershipType.IsValid(membershipType))
            {
                return new Result
                {
                    Success = false,
                    Message = _ctx.Text["유효하지 않은 멤버십 유형입니다.|Invalid membership type."]
                };
            }

            var customer = await db.Repo<Customer>()
                .GetOneEdit(x => x.Guid == guid);

            if (customer == null)
            {
                return new Result
                {
                    Success = false,
                    Message = _ctx.Text["고객을 찾을 수 없습니다.|Customer was not found."]
                };
            }

            customer.MembershipType = membershipType;
            customer.DateModified = DateTime.UtcNow;

            await db.Repo<Customer>().Update(customer);

            return new Result
            {
                Success = true,
                Message = _ctx.Text["멤버십이 변경되었습니다.|Membership has been updated."]
            };
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error setting membership. Guid={Guid}", guid);

            return new Result
            {
                Success = false,
                Message = _ctx.Text["멤버십 변경 중 오류가 발생했습니다.|An error occurred while updating membership."]
            };
        }
    }
    public async Task<ResultOf<Customer>> GetCustomerAsync(Guid guid)
    {
        using var db = _ctx.ConnectDb();

        try
        {
            var customer = await db.Repo<Customer>()
                .Query()
                .FirstOrDefaultAsync(x => x.Guid == guid);

            if (customer == null)
            {
                return ResultOf<Customer>.Error(
                    _ctx.Text["고객을 찾을 수 없습니다.|Customer was not found."]
                );
            }

            return ResultOf<Customer>.Ok(customer);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error getting customer detail. Guid={Guid}", guid);

            return ResultOf<Customer>.Error(
                _ctx.Text["고객 정보를 불러오는 중 오류가 발생했습니다.|An error occurred while loading customer detail."]
            );
        }
    }
}