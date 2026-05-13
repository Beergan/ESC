using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESC.CONCOST.Abstract
{
    [Table("INSURANCE_RATES")]
    public class InsuranceRate : EntityBase
    {
        
        [Column("accident_rate")]
        public decimal AccidentRate { get; set; }

        [Column("employment_rate")]
        public decimal EmploymentRate { get; set; }

        [Column("retirement_rate")]
        public decimal RetirementRate { get; set; }

        [Column("health_rate")]
        public decimal HealthRate { get; set; }

        [Column("pension_rate")]
        public decimal PensionRate { get; set; }

        [Column("care_rate")]
        public decimal CareRate { get; set; }

        [Column("base_wage")]
        public long BaseWage { get; set; }
    }
}
