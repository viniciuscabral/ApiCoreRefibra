using Microsoft.EntityFrameworkCore;


namespace ApiRefibra.Repo
{
    public class ApplicationDbContext : DbContext
    {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext> context) : base(context)
        {

        }
    }
}
