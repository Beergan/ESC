using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleEmployeeCore;

namespace ESC.CONCOST.ModuleEmployee;

public class EntityRegister : IEntityRegister
{
    public void RegisterEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EntityEmployee>().HasAlternateKey(k => k.Guid);
    }
    public void Seed(IDbContext db)
    {
        
    }
}