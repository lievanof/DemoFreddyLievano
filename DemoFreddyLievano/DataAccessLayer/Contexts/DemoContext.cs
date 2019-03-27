using Demo.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Demo.DataAccessLayer.Context
{
    public class DemoContext: DbContext
    {

        #region constructor

        public DemoContext()
            : base("MyConnection")
        { }

        #endregion

        #region properties

        public DbSet<User> Users { get; set; }
        public DbSet<UserData> UserData { get; set; }
        public DbSet<Session> Sessions { get; set; }

        #endregion

        #region methods

        public static DemoContext Create()
        {
            return new DemoContext();
        }

        public virtual void Commit()
        {
            base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<UserData>()
                .HasRequired(u => u.User)
                .WithOptional(u => u.UserData);

            modelBuilder.Entity<Session>()
                .HasRequired(u => u.User)
                .WithOptional(u => u.Session);                        
        }

        #endregion

    }
}
