using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Blazor.RCL.Domain.Entities.Configuration;
using Blazor.RCL.Automation.AutomationRequest;

namespace Blazor.RCL.Automation.Data
{
    public class AutomationRequestDbContext : DbContext
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AppDbContext class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public AutomationRequestDbContext(DbContextOptions<AutomationRequestDbContext> options) : base(options) { }

        #endregion

        #region DbSet Properties

        /// <summary>
        /// Gets or sets the RequestLog entities.
        /// </summary>
        public DbSet<RequestLog> RequestLogs { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new instance of AppDbContext.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        /// <returns>A new instance of AppDbContext.</returns>
        public static AutomationRequestDbContext Create(DbContextOptions<AutomationRequestDbContext> options)
        {
            return new AutomationRequestDbContext(options);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Configures the model that was discovered by convention from the entity types exposed in DbSet properties on your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the RequestLog entity to match the database schema
            modelBuilder.Entity<RequestLog>(entity =>
            {
                // Map to the correct table in the dbo schema
                entity.ToTable("RequestLog", "dbo");
                
                // Configure primary key
                entity.HasKey(e => e.RequestLogPK);
                
                // Configure properties based on the schema
                entity.Property(e => e.Source).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.SourceId).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.Request).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.RequestItem).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.CatalogItem).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.AccessType).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.AccessSubtype).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.RequestStatusComments).IsUnicode(true);
                entity.Property(e => e.AuditAddUserName).HasMaxLength(100).IsUnicode(false).IsRequired();
                entity.Property(e => e.AuditChangeUserName).HasMaxLength(100).IsUnicode(false).IsRequired();
            });
        }

        #endregion
    }
}
