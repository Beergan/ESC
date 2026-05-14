using ESC.CONCOST.Abstract;
using RestEase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleSettingCore;

public interface IEscFormulaSettingService
{
    Task<List<EscFormulaSetting>> GetFormulasAsync();

    Task<EscFormulaSetting?> GetFormulaWithFieldsAsync(Guid formulaGuid);

    Task<EscFormulaSetting?> GetDefaultFormulaWithFieldsAsync();

    Task<Result> SaveFormulaAsync(EscFormulaSetting formula);

    Task<Result> SetDefaultAsync(Guid formulaGuid);

    Task<Result> DeleteFormulaAsync(Guid formulaGuid);

    Task<Result> SaveFieldAsync(EscFormulaField field);

    Task<Result> DeleteFieldAsync(Guid fieldGuid);

    Task<Result> SaveFieldOptionAsync(EscFormulaFieldOption option);

    Task<Result> DeleteFieldOptionAsync(Guid optionGuid);

    Task<ResultOf<decimal>> TestFormulaAsync(Guid formulaGuid, Dictionary<string, string> values);
}