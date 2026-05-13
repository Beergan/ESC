using Microsoft.EntityFrameworkCore;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.Base;

public class EntityRegister : IEntityRegister
{
    public void RegisterEntities(ModelBuilder modelBuilder)
    {

    }

    public void Seed(IDbContext db)
    {
    }
}