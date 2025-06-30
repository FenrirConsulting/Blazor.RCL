using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Blazor.RCL.Domain.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Blazor.RCL.Domain.Entities.Configuration;
using Blazor.RCL.Domain.Entities.Authentication;
using Blazor.RCL.Domain.Entities.Notifications;
using Microsoft.Extensions.Logging;

namespace Blazor.RCL.Infrastructure.Data
{
    /// <summary>
    /// Represents the database context for the application, extending IdentityDbContext.
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AppDbContext class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        #endregion

        #region DbSet Properties

        /// <summary>
        /// Gets or sets the UserSettings DbSet.
        /// </summary>
        public DbSet<UserSettings> UserSettings { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DistributedCache DbSet.
        /// </summary>
        public DbSet<DistributedCacheEntry> DistributedCache { get; set; } = null!;

        /// <summary>
        // Add this new DbSet for DataProtection
        /// <summary>
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ToolsConfiguration DbSet.
        /// </summary>
        public DbSet<ToolsConfiguration> ToolsConfiguration { get; set; } = null!;

        /// <summary>
        /// Gets or sets the ToolsConfiguration DbSet.
        /// </summary>
        public DbSet<LDAPServer> LDAPServers { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for RemoteScripts.
        /// </summary>
        public DbSet<RemoteScript> RemoteScripts { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for ServiceAccounts.
        /// </summary>
        public DbSet<ServiceAccount> ServiceAccounts { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for ServiceAccounts.
        /// </summary>
        public DbSet<ServerHost> ServerHosts { get; set; }

        /// <summary>
        /// Gets or sets the DbSet for ToolsRequest.
        /// </summary>
        public DbSet<ToolsRequest> ToolsRequest { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for RequestStatusCodes.
        /// </summary>
        public DbSet<RequestStatusCode> RequestStatusCodes { get; set; } = null!;

        #region Notification DbSets

        /// <summary>
        /// Gets or sets the DbSet for NotificationMessages.
        /// </summary>
        public DbSet<NotificationMessage> NotificationMessages { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for UserNotificationSettings.
        /// </summary>
        public DbSet<UserNotificationSettings> UserNotificationSettings { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for ApplicationNotificationProfiles.
        /// </summary>
        public DbSet<ApplicationNotificationProfile> ApplicationNotificationProfiles { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for UserApplicationNotificationSettings.
        /// </summary>
        public DbSet<UserApplicationNotificationSettings> UserApplicationNotificationSettings { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for NotificationDeliveries.
        /// </summary>
        public DbSet<NotificationDelivery> NotificationDeliveries { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for EmailNotificationQueues.
        /// </summary>
        public DbSet<EmailNotificationQueue> EmailNotificationQueues { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for EmailTemplates.
        /// </summary>
        public DbSet<EmailTemplate> EmailTemplates { get; set; } = null!;

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new instance of AppDbContext.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        /// <returns>A new instance of AppDbContext.</returns>
        public static AppDbContext Create(DbContextOptions<AppDbContext> options)
        {
            return new AppDbContext(options);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Configures the database provider and connection string to be used for this context.
        /// </summary>
        /// <param name="optionsBuilder">A builder used to create or modify options for this context.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // This will be skipped if options already configured in the constructor
                return;
            }
            
            optionsBuilder
                .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
                .EnableSensitiveDataLogging();
        }

        /// <summary>
        /// Configures the model that was discovered by convention from the entity types exposed in DbSet properties on your derived context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DataProtectionKey>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.ToTable(tb => tb.HasTrigger("TR_DataProtectionKeys_Update"));
            });
                
            modelBuilder.Entity<DistributedCacheEntry>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.ToTable(tb => tb.HasTrigger("TR_DistributedCache_Update"));
            });
                
            modelBuilder.Entity<LDAPServer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.ToTable(tb => tb.HasTrigger("TR_LDAPServers_Update"));
            });
                
            modelBuilder.Entity<RemoteScript>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.ToTable(tb => tb.HasTrigger("TR_RemoteScripts_Update"));
            });
                
            modelBuilder.Entity<RequestStatusCode>(entity =>
            {
                entity.HasKey(e => e.RequestStatusCodePK);
                entity.Property(e => e.RequestStatusCodePK).ValueGeneratedOnAdd();
                entity.ToTable(tb => tb.HasTrigger("TR_RequestStatusCodes_Update"));
            });
                
            modelBuilder.Entity<ServerHost>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.ToTable(tb => tb.HasTrigger("TR_ServerHosts_Update"));
            });
                
            modelBuilder.Entity<ServiceAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.ToTable(tb => tb.HasTrigger("TR_ServiceAccounts_Update"));
            });
                
            modelBuilder.Entity<ToolsConfiguration>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.ToTable(tb => tb.HasTrigger("TR_ToolsConfiguration_Update"));
            });
                
            modelBuilder.Entity<ToolsRequest>(entity =>
            {
                entity.HasKey(e => e.ToolsRequestPK);
                entity.Property(e => e.ToolsRequestPK).ValueGeneratedOnAdd();
                entity.ToTable(tb => tb.HasTrigger("TR_ToolsRequest_Update"));
            });
                
            modelBuilder.Entity<UserSettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.ToTable(tb => tb.HasTrigger("TR_UserSettings_Update"));
                
                // Index for role-based queries (Phase 2.8)
                entity.HasIndex(e => e.Roles)
                    .HasFilter("[Roles] IS NOT NULL")
                    .HasDatabaseName("IX_UserSettings_Roles");
            });

            #region Notification Entity Configurations

            modelBuilder.Entity<NotificationMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever(); // We generate GUIDs in code
                entity.HasIndex(e => e.MessageId).IsUnique();
                entity.HasIndex(e => e.SourceApplication);
                entity.HasIndex(e => new { e.CreatedAt, e.IsActive });
                entity.ToTable("NotificationMessage");
            });

            modelBuilder.Entity<UserNotificationSettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasOne(e => e.UserSettings)
                    .WithMany()
                    .HasForeignKey(e => e.Username)
                    .HasPrincipalKey(u => u.Username)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.ToTable("UserNotificationSettings");
            });

            modelBuilder.Entity<ApplicationNotificationProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.ApplicationName).IsUnique();
                entity.ToTable("ApplicationNotificationProfile");
            });

            modelBuilder.Entity<UserApplicationNotificationSettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => new { e.Username, e.ApplicationName }).IsUnique();
                entity.HasOne(e => e.UserNotificationSettings)
                    .WithMany()
                    .HasForeignKey(e => e.Username)
                    .HasPrincipalKey(u => u.Username)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.ApplicationNotificationProfile)
                    .WithMany()
                    .HasForeignKey(e => e.ApplicationName)
                    .HasPrincipalKey(a => a.ApplicationName)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.ToTable("UserApplicationNotificationSettings");
            });

            modelBuilder.Entity<NotificationDelivery>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => e.NotificationId);
                entity.HasIndex(e => new { e.Username, e.DeliveryStatus });
                entity.HasOne(e => e.NotificationMessage)
                    .WithMany()
                    .HasForeignKey(e => e.NotificationId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.UserNotificationSettings)
                    .WithMany()
                    .HasForeignKey(e => e.Username)
                    .HasPrincipalKey(u => u.Username)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.ToTable("NotificationDelivery");
            });

            modelBuilder.Entity<EmailNotificationQueue>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => new { e.Status, e.ScheduledSendTime });
                entity.HasIndex(e => e.Username);
                entity.HasOne(e => e.NotificationMessage)
                    .WithMany()
                    .HasForeignKey(e => e.NotificationId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.ToTable("EmailNotificationQueue");
            });

            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasIndex(e => new { e.TemplateKey, e.ApplicationName, e.IsActive });
                entity.HasIndex(e => e.ApplicationName);
                entity.HasOne(e => e.Application)
                    .WithMany()
                    .HasForeignKey(e => e.ApplicationName)
                    .HasPrincipalKey(a => a.ApplicationName)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.ToTable("EmailTemplate");
            });

            #endregion
        }

        #endregion
    }
}