using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Prng;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.ModuleSettingCore;

namespace ESC.CONCOST.ModuleSetting;

public class EntityRegister : IEntityRegister
{
    public void RegisterEntities(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EntityNotification>().HasAlternateKey(x => x.Guid);
        modelBuilder.Entity<EntityPermission>().HasAlternateKey(x => x.Guid);
        modelBuilder.Entity<EntityNotification>()
           .Property(e => e.Guid_UserNotification)
           .HasConversion(
               v => Newtonsoft.Json.JsonConvert.SerializeObject(v),
               v => Newtonsoft.Json.JsonConvert.DeserializeObject<Guid[]>(v)
           );
        modelBuilder.Entity<EscFormulaSetting>().HasAlternateKey(x => x.Guid);
        modelBuilder.Entity<EscFormulaVariable>().HasAlternateKey(x => x.Guid);
        modelBuilder.Entity<EscFormulaHistory>().HasAlternateKey(x => x.Guid);
        modelBuilder.Entity<IndexType>().HasAlternateKey(x => x.Guid);
        modelBuilder.Entity<IndexTimeSeries>().HasAlternateKey(x => x.Guid);
    }
    public void Seed(IDbContext db)
    {
        // No default seeding required for lean architecture
    }
}