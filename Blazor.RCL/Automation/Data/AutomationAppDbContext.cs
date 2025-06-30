using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Blazor.RCL.Domain.Entities.Configuration;

namespace Blazor.RCL.Automation.Data
{
    public class AutomationAppDbContext : DbContext
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AppDbContext class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public AutomationAppDbContext(DbContextOptions<AutomationAppDbContext> options) : base(options) { }

        #endregion

        #region DbSet Properties

        /// <summary>
        /// Populate this with DBSet Properties (Tables)
        /// </summary>

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new instance of AppDbContext.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        /// <returns>A new instance of AppDbContext.</returns>
        public static AutomationAppDbContext Create(DbContextOptions<AutomationAppDbContext> options)
        {
            return new AutomationAppDbContext(options);
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
