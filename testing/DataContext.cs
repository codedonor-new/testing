using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace testing
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        // DbSet properties for your entities
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<TestUser> TestUsers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Person> Persons { get; set; }
        public override int SaveChanges()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified)
                .Select(x => x.Entity);

            var currentDate = DateTime.UtcNow; 

            foreach (var entity in entities)
            {
                if (entity is CommanEntity commonEntity)
                {
                    if (this.Entry(commonEntity).State == EntityState.Added)
                    {
                        int currentUserId = GetCurrentUserId();
                        commonEntity.CreatedId = currentUserId;
                        commonEntity.CreatedDate = currentDate;
                    }

                    if (this.Entry(commonEntity).State == EntityState.Modified)
                    {
                        int currentUserId = GetCurrentUserId();

                        commonEntity.UpdatedId = currentUserId;
                        commonEntity.UpdatedDate = currentDate;
                    }
                }
            }

            return base.SaveChanges();
        }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
                {
                    var isDeletedProperty = entityType.ClrType.GetProperty("IsDeleted");

                    if (isDeletedProperty != null)
                    {
                        var parameter = Expression.Parameter(entityType.ClrType, "e");
                        var property = Expression.PropertyOrField(parameter, isDeletedProperty.Name);
                        var condition = Expression.Equal(property, Expression.Constant(false));
                        var lambda = Expression.Lambda(condition, parameter);

                        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                    }
                }
            }

            //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            //    {
            //        var parameter = Expression.Parameter(typeof(CommanEntity), "e"); // Change the type here
            //        var property = Expression.PropertyOrField(parameter, "IsDeleted");
            //        var condition = Expression.Equal(property, Expression.Constant(false));
            //        var lambda = Expression.Lambda(condition, parameter);

            //        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            //    }
            //}
        }

        private int GetCurrentUserId()
        {
            // Implement the logic to retrieve the current user's ID from token.
            //var userid = Convert.ToInt32(HttpContext.User.FindFirstValue("Id"));
            return 1; 
        }
    }
}
