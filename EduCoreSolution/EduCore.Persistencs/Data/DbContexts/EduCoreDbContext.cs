using EduCore.Domain.Entities.CourseModel;
using EduCore.Persistencs.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Persistencs.Data.DbContexts
{
    public class EduCoreDbContext : DbContext
    {
        public EduCoreDbContext(DbContextOptions<EduCoreDbContext> options) :base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            //Access To All Configrations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourseConfiguration).Assembly);
        }

        #region Dbsets
        public DbSet<Course> courses { get; set; }
        #endregion
    }
}
