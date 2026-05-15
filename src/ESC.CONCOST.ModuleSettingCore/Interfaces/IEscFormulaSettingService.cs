using ESC.CONCOST.Abstract;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleSetting;

/// <summary>
/// Contract cho EscFormulaSettingService.
/// Tất cả method public đều phải khai báo ở đây.
/// </summary>
public interface IEscFormulaSettingService
{
    // ── Queries ──────────────────────────────────────────────

    /// <param name="includeInactive">true = trả về cả record đã soft-delete/disable</param>
    Task<List<EscFormulaSetting>> GetListAsync(bool includeInactive = false);

    Task<EscFormulaSetting?> GetCurrentAsync();

    Task<EscFormulaSetting?> GetByGuidAsync(Guid guid);

    Task<List<EscFormulaVariable>> GetVariablesAsync();

    // ── Commands ─────────────────────────────────────────────

    /// <summary>
    /// Tạo mới hoặc cập nhật formula setting.
    /// Tự động tạo audit history khi update.
    /// </summary>
    Task<ResultOf<EscFormulaSetting>> SaveAsync(EscFormulaSetting model, string changeNote);

    /// <summary>Đặt formula này làm current, bỏ current flag của tất cả formula khác.</summary>
    Task<ResultOf<bool>> SetCurrentAsync(Guid guid);

    /// <summary>Vô hiệu hóa formula (IsActive = false). Không thể disable formula đang là current.</summary>
    Task<ResultOf<bool>> DisableAsync(Guid guid);

    /// <summary>
    /// Soft delete: ghi audit history rồi set IsActive = false, IsCurrent = false.
    /// Không thể xóa formula đang là current hoặc là default.
    /// </summary>
    Task<ResultOf<bool>> DeleteAsync(Guid guid, string deleteReason);

    // ── Validate & Test ──────────────────────────────────────

    /// <summary>Validate công thức: field bắt buộc, expression an toàn, variable hợp lệ.</summary>
    Task<ResultOf<bool>> ValidateFormulaAsync(EscFormulaSetting model);

    /// <summary>Chạy thử công thức với input mẫu, trả về từng bước tính toán.</summary>
    Task<ResultOf<FormulaTestResultDto>> TestFormulaAsync(EscFormulaSetting model, FormulaTestInputDto input);
}