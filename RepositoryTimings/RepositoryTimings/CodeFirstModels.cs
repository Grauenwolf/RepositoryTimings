
using System.Data.Entity;

namespace RepositoryTimings
{
    public partial class CodeFirstModels : DbContext
    {
        public CodeFirstModels()
            : base("name=CodeFirstModels")
        {
        }

        public virtual DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Employee1)
                .WithOptional(e => e.Employee2)
                .HasForeignKey(e => e.ManagerKey);

        }
    }
}
