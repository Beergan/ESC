using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.ModuleESCCore.Models
{
    public class ContractWizardDto
    {
        // ==========================================
        // STEP 1: 기본정보 (Basic Info)
        // ==========================================
        [Required(ErrorMessage = "공사명을 입력해주세요.|Please enter project name.")]
        public string ProjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "시공사명을 입력해주세요.|Please enter contractor name.")]
        public string Contractor { get; set; } = string.Empty;

        [Required(ErrorMessage = "발주처명을 입력해주세요.|Please enter client name.")]
        public string Client { get; set; } = string.Empty;

        [Required(ErrorMessage = "공사종류를 입력해주세요.|Please enter work type.")]
        public string WorkType { get; set; } = string.Empty;

        [Required(ErrorMessage = "계약방법을 입력해주세요.|Please enter contract method.")]
        public string ContractMethod { get; set; } = string.Empty;

        [Required(ErrorMessage = "낙찰율을 입력해주세요.|Please enter bid rate.")]
        [Range(0.01, 100.0, ErrorMessage = "올바른 낙찰율을 입력해주세요.|Invalid bid rate.")]
        public decimal BidRate { get; set; }

        public string PreparedBy { get; set; } = string.Empty;


        // ==========================================
        // STEP 2: 계약 금액·일정 (Contract Amount & Schedule)
        // ==========================================
        [Required(ErrorMessage = "계약금액을 입력해주세요.|Please enter contract amount.")]
        [Range(1, long.MaxValue, ErrorMessage = "계약금액은 0보다 커야 합니다.|Contract amount must be greater than 0.")]
        public long ContractAmount { get; set; }

        [Required(ErrorMessage = "계약체결일을 선택해주세요.|Please select contract date.")]
        public DateTime? ContractDate { get; set; }

        [Required(ErrorMessage = "입찰일을 선택해주세요.|Please select bid date.")]
        public DateTime? BidDate { get; set; }

        [Required(ErrorMessage = "착공일을 선택해주세요.|Please select start date.")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "준공예정일을 선택해주세요.|Please select completion date.")]
        public DateTime? CompletionDate { get; set; }


        // ==========================================
        // STEP 3: ESC 조정 설정 (ESC Settings)
        // ==========================================
        [Required(ErrorMessage = "조정기준일을 선택해주세요.|Please select adjustment base date.")]
        public DateTime? CompareDate { get; set; }

        [Required(ErrorMessage = "등락율 기준을 입력해주세요.|Please enter fluctuation rate threshold.")]
        public decimal ThresholdRate { get; set; } = 3.0m; // 3%

        [Required(ErrorMessage = "경과기간 기준을 입력해주세요.|Please enter time threshold.")]
        public int ThresholdDays { get; set; } = 90; // 90 days

        public long AdvanceAmt { get; set; }
        
        public long ExcludedAmt { get; set; }

        public string PreviousMonth { get; set; } = string.Empty;

        /// <summary>
        /// Converts DTO to Contract entity
        /// </summary>
        public Contract ToEntity(int? customerId)
        {
            return new Contract
            {
                CustomerId = customerId,
                ProjectName = ContractWizardValidator.NormalizeText(this.ProjectName, 255),
                Contractor = ContractWizardValidator.NormalizeText(this.Contractor, 255),
                Client = ContractWizardValidator.NormalizeText(this.Client, 255),
                WorkType = ContractWizardValidator.NormalizeText(this.WorkType, 50),
                ContractMethod = ContractWizardValidator.NormalizeText(this.ContractMethod, 50),
                BidRate = this.BidRate,
                ContractAmount = this.ContractAmount,
                ContractDate = this.ContractDate ?? DateTime.Now,
                BidDate = this.BidDate ?? DateTime.Now,
                StartDate = this.StartDate ?? DateTime.Now,
                CompletionDate = this.CompletionDate ?? DateTime.Now,
                CompareDate = this.CompareDate ?? DateTime.Now,
                ThresholdRate = this.ThresholdRate / 100m, // Store as decimal ratio
                ThresholdDays = this.ThresholdDays,
                AdvanceAmt = this.AdvanceAmt,
                ExcludedAmt = this.ExcludedAmt,
                PreparedBy = ContractWizardValidator.NormalizeText(this.PreparedBy, 255),
                PreviousMonth = ContractWizardValidator.NormalizeText(this.PreviousMonth, 7)
            };
        }
    }
}
