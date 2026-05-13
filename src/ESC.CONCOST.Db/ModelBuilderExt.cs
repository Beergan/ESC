using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.Db;

public static class ModelBuilderExt
{
    /// <summary>
    /// Lưu ý: Dữ liệu mẫu hiện đã được chuyển sang StartupUtil.cs để nạp động (Dynamic Seeding)
    /// Điều này giúp tránh việc Migration bị phình to và cho phép quản lý dữ liệu linh hoạt hơn.
    /// </summary>
    public static void SeedData(this ModelBuilder builder)
    {
        // Hiện tại không dùng HasData để nạp dữ liệu mẫu nữa.
        // Dữ liệu tổ chức và nhân sự mẫu được nạp tại StartupUtil.SeedOrganizationData
    }
}