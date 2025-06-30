using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Blazor.RCL.Domain.Entities.Configuration;
using Blazor.RCL.Automation.AutomationDirectory;

namespace Blazor.RCL.Automation.Data
{
    public class AutomationBatchDbContext : DbContext
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AppDbContext class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public AutomationBatchDbContext(DbContextOptions<AutomationBatchDbContext> options) : base(options) { }

        #endregion

        #region DbSet Properties

        /// <summary>
        /// Gets or sets the AutomationBatchADAcctDispositionActions DbSet.
        /// </summary>

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new instance of AppDbContext.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        /// <returns>A new instance of AppDbContext.</returns>
        public static AutomationBatchDbContext Create(DbContextOptions<AutomationBatchDbContext> options)
        {
            return new AutomationBatchDbContext(options);
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

            // Add any additional model configurations here
            // For example:
            // modelBuilder.Entity<UserSettings>().HasIndex(u => u.UserId).IsUnique();
        }
        #endregion
    }
}
