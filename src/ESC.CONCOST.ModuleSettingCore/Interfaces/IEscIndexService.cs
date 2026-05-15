using ESC.CONCOST.Abstract;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleSetting;

public interface IEscIndexService
{
    Task<List<EscIndexGroup>> GetIndexGroupsAsync(bool includeInactive = false);

    Task<EscIndexGroup?> GetIndexGroupByIdAsync(int id);

    Task<ResultOf<EscIndexGroup>> SaveIndexGroupAsync(EscIndexGroup model);

    Task<ResultOf<bool>> ToggleGroupActiveAsync(int id);

    Task<ResultOf<bool>> DeleteGroupAsync(int id);

    Task<List<IndexType>> GetIndexTypesAsync(bool includeInactive = false, string groupCode = null);

    Task<IndexType?> GetIndexTypeByIdAsync(int id);

    Task<ResultOf<IndexType>> SaveIndexTypeAsync(IndexType model);

    Task<ResultOf<bool>> ToggleActiveAsync(int id);

    Task<ResultOf<bool>> DeleteIndexTypeAsync(int id);

    Task<List<IndexTimeSeries>> GetTimeSeriesAsync(int? year = null, string groupCode = null);

    Task<List<string>> GetAvailablePeriodsAsync();

    Task<ResultOf<bool>> SaveTimeSeriesBatchAsync(List<IndexTimeSeries> entries);

    Task<ResultOf<bool>> AddMonthsAsync(string fromPeriodKey, string toPeriodKey);

    Task<ResultOf<bool>> CopyFromPreviousMonthAsync(string periodKey);

    Task<ResultOf<bool>> DeleteMonthAsync(string periodKey);


}