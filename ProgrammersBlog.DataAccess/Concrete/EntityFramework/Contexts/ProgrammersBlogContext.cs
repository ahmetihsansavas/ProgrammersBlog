using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.DataAccess.Concrete.EntityFramework.Mappings;
using ProgrammersBlog.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammersBlog.DataAccess.Concrete.EntityFramework.Contexts
{
    public  class ProgrammersBlogContext : IdentityDbContext<User,Role,int,UserClaim,UserRole,UserLogin,RoleClaim,UserToken> //User ,Role ve primary key türünü ver.
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString: @"Server=(localdb)\MSSQLLocalDB;Database=ProgrammersBlogDb;Trusted_Connection=True;
                                                            Timeout=60;MultipleActiveResultSets=True;");
        }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
      //  public DbSet<Role> Role { get; set; }
      //  public DbSet<User> Users { get; set; } Bu classları .net core kütüp. olan Identity Core dan alıyoruz.

        protected override void OnModelCreating(ModelBuilder modelBuilder) //Db oluş zaman uygulanacak Mapping işlem. ayarlıyoruz..,Db-First ile de Kullanılab.
        {
            modelBuilder.ApplyConfiguration(new ArticleMap());
            modelBuilder.ApplyConfiguration(new CategoryMap());
            modelBuilder.ApplyConfiguration(new CommentMap());
            modelBuilder.ApplyConfiguration(new RoleMap());
            modelBuilder.ApplyConfiguration(new UserMap());
        }

    }
}
