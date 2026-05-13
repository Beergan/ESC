using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Prng;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleManagementCore;

namespace ESC.CONCOST.ModuleManagement;

public class EntityRegister : IEntityRegister
{
    public void RegisterEntities(ModelBuilder modelBuilder)
    {
    }
    public void Seed(IDbContext db)
    {
    }
}