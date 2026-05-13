using ESC.CONCOST.ModuleESCCore.Models;

namespace ESC.CONCOST.ModuleESCCore.Tests;

public class ContractWizardValidatorTests
{
    [Fact]
    public void Validate_WithValidModel_ReturnsValid()
    {
        var model = CreateValidModel();

        var result = ContractWizardValidator.Validate(model);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WhenRequiredFieldsAreMissing_ReturnsStep1Errors()
    {
        var model = CreateValidModel();
        model.ProjectName = "";
        model.WorkType = " ";

        var result = ContractWizardValidator.Validate(model, 1);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.Contains("공사명"));
        Assert.Contains(result.Errors, x => x.Contains("공사 종류"));
    }

    [Fact]
    public void Validate_WhenAdvancePaymentExceedsContractAmount_ReturnsError()
    {
        var model = CreateValidModel();
        model.ContractAmount = 1000;
        model.AdvanceAmt = 1001;

        var result = ContractWizardValidator.Validate(model, 2);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.Contains("선금액"));
    }

    [Fact]
    public void Validate_WhenCompletionDateIsBeforeStartDate_ReturnsError()
    {
        var model = CreateValidModel();
        model.StartDate = new DateTime(2026, 5, 10);
        model.CompletionDate = new DateTime(2026, 5, 10);

        var result = ContractWizardValidator.Validate(model, 2);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.Contains("준공일"));
    }

    [Fact]
    public void Validate_WhenCompareDateIsBeforeThresholdDays_ReturnsError()
    {
        var model = CreateValidModel();
        model.BidDate = new DateTime(2026, 1, 1);
        model.CompareDate = new DateTime(2026, 3, 1);
        model.ThresholdDays = 90;

        var result = ContractWizardValidator.Validate(model, 3);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, x => x.Contains("경과기간"));
    }

    [Theory]
    [InlineData("2026-05", true)]
    [InlineData("", true)]
    [InlineData("2026-5", false)]
    [InlineData("2026/05", false)]
    public void IsValidYearMonth_ValidatesExpectedFormat(string value, bool expected)
    {
        var result = ContractWizardValidator.IsValidYearMonth(value);

        Assert.Equal(expected, result);
    }

    private static ContractWizardDto CreateValidModel()
    {
        return new ContractWizardDto
        {
            ProjectName = "ESC 테스트 공사",
            Contractor = "컨코스트 건설",
            Client = "한국수자원공사",
            WorkType = "전기",
            ContractMethod = "계속비",
            BidRate = 87.5m,
            PreparedBy = "주식회사 컨코스트",
            ContractAmount = 1_250_000_000,
            AdvanceAmt = 0,
            ContractDate = new DateTime(2026, 1, 15),
            BidDate = new DateTime(2026, 1, 1),
            StartDate = new DateTime(2026, 2, 1),
            CompletionDate = new DateTime(2027, 2, 1),
            CompareDate = new DateTime(2026, 5, 1),
            ThresholdRate = 3.0m,
            ThresholdDays = 90,
            ExcludedAmt = 0,
            PreviousMonth = "2026-04"
        };
    }
}
