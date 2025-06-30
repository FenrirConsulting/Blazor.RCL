using System;
using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Domain.Entities.Configuration
{
    /// <summary>
    /// Represents user settings in the application.
    /// This entity defines the structure for the UserSettings table in the database.
    /// </summary>
    public class UserSettings
    {
        #region Properties

        /// <summary>
        /// Unique identifier for the user settings.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Username associated with these settings.
        /// </summary>
        [Required]
        [MaxLength(256)]
        public string? Username { get; set; }

        /// <summary>
        /// Indicates whether dark mode is enabled for the user.
        /// </summary>
        [MaxLength(10)]
        public string? DarkMode { get; set; }

        /// <summary>
        /// Stores additional user-specific settings as a string (consider using JSON for complex settings).
        /// </summary>
        public string? AdditionalSettings { get; set; }

        /// <summary>
        /// JSON array of user roles captured from authentication claims.
        /// Used for role-based notification targeting.
        /// </summary>
        [MaxLength(2000)]
        public string? Roles { get; set; }

        /// <summary>
        /// Timestamp of when roles were last updated from authentication claims.
        /// Used to determine if roles need to be refreshed.
        /// </summary>
        public DateTime? RolesLastUpdated { get; set; }

        #endregion
    }
}