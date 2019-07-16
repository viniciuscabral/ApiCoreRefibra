using Microsoft.EntityFrameworkCore;


namespace WsTestes.Repo
{
    public class ApplicationDbContext : DbContext
    {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext> context) : base(context)
        {

        }
    }
}
