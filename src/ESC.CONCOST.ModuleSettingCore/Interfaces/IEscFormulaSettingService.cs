using ESC.CONCOST.Abstract;
using RestEase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESC.CONCOST.ModuleSettingCore;

public interface IEscFormulaSettingService
{
    Task<List<EscFormulaSetting>> GetFormulasAsync();

    Task<EscFormulaSetting?> GetFormulaAsync(Guid formulaGuid);

    Task<EscFormulaSetting?> GetActiveFormulaAsync();

    Task<Result> SaveFormulaAsync(EscFormulaSetting formula);

    Task<Result> SetActiveAsync(Guid formulaGuid);

    Task<Result> DeleteFormulaAsync(Guid formulaGuid);

    Task<List<EscFormulaVariable>> GetVariablesAsync();

    Task<Result> SaveVariableAsync(EscFormulaVariable variable);

    Task<Result> DeleteVariableAsync(Guid variableGuid);

    Task<List<EscFormulaHistory>> GetFormulaHistoryAsync(int formulaSettingId);

    Task<ResultOf<Dictionary<string, decimal>>> TestFormulaAsync(EscFormulaSetting formula, Dictionary<string, decimal> inputValues);
}