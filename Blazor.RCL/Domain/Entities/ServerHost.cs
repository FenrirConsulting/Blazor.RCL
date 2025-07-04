﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Domain.Entities
{
    /// <summary>
    /// Represents a server host configuration entry.
    /// </summary>
    public class ServerHost
    {
        #region Properties

        /// <summary>
        /// Gets or sets the identifier for the server host.
        /// </summary>
        [Key]
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the hostname of the server.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Hostname { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the environment designation (e.g., DEV, QA, PROD).
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Environment { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name for the server.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the role of the server.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user who added the record.
        /// </summary>
        public string AuditAddBy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the record was added.
        /// </summary>
        public DateTime AuditAddDate { get; set; }

        /// <summary>
        /// Gets or sets the user who last modified the record.
        /// </summary>
        public string? AuditChangeBy { get; set; }

        /// <summary>
        /// Gets or sets the date when the record was last modified.
        /// </summary>
        public DateTime? AuditChangeDate { get; set; }

        #endregion
    }
}
