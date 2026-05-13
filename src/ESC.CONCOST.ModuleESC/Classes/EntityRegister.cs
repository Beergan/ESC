using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleESCCore;
using ESC.CONCOST.Abstract.Entities;

namespace ESC.CONCOST.ModuleESC;

public class EntityRegister : IEntityRegister
{
    public void RegisterEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConstructionCategory>();
        modelBuilder.Entity<ContractCategory>();
        modelBuilder.Entity<Contract>();
        modelBuilder.Entity<ContractItem>();
        modelBuilder.Entity<AdjustRecord>();
        modelBuilder.Entity<AdjustItemDetail>();
        modelBuilder.Entity<EscServiceRequest>();
    }
    public void Seed(IDbContext db)
    {
        
    }
}
