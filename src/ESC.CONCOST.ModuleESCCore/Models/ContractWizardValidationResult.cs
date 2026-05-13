using System.Collections.Generic;
using System.Linq;

namespace ESC.CONCOST.ModuleESCCore.Models;

public sealed class ContractWizardValidationResult
{
    public ContractWizardValidationResult(IEnumerable<string> errors)
    {
        Errors = errors
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }

    public IReadOnlyList<string> Errors { get; }

    public bool IsValid => Errors.Count == 0;

    public string Message => string.Join("\n", Errors);
}
