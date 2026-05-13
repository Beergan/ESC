using Microsoft.EntityFrameworkCore;

namespace ESC.CONCOST.Base;

public interface IEntityRegister
{
    void RegisterEntities(ModelBuilder modelbuilder);

    void Seed(IDbContext db);
}