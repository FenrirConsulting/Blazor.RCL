using System;
using System.Collections.Generic;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Models.Notifications
{
    /// <summary>
    /// Defines starter email templates for quick setup
    /// </summary>
    public static class StarterTemplates
    {
        public class StarterTemplate
        {
            public string Key { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
            public string IconColor { get; set; } = "Primary";
            public string SubjectTemplate { get; set; } = string.Empty;
            public string HtmlBodyTemplate { get; set; } = string.Empty;
            public string TextBodyTemplate { get; set; } = string.Empty;
            public List<AlertType> SupportedAlertTypes { get; set; } = new();
            public string Category { get; set; } = string.Empty;
        }

        public static readonly List<StarterTemplate> Templates = new()
        {
            // System Alert Template
            new StarterTemplate
            {
                Key = "SYSTEM_ALERT",
                Name = "System Alert",
                Description = "General system notifications with severity indicators",
                Icon = "Material.Filled.NotificationsActive",
                IconColor = "Primary",
                Category = "Alerts",
                SupportedAlertTypes = new List<AlertType> { AlertType.System },
                SubjectTemplate = "[{{Severity}}] {{Title}} - {{ApplicationDisplayName}}",
                HtmlBodyTemplate = @"<!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; line-height: 1.6; color: #333; }
                        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; border-radius: 8px 8px 0 0; }
                        .severity-badge { display: inline-block; padding: 4px 12px; border-radius: 16px; font-size: 14px; font-weight: 600; }
                        .severity-critical { background-color: #dc3545; color: white; }
                        .severity-error { background-color: #dc3545; color: white; }
                        .severity-warning { background-color: #ffc107; color: #333; }
                        .severity-info { background-color: #0d6efd; color: white; }
                        .content { background: #ffffff; padding: 30px; border: 1px solid #e0e0e0; border-radius: 0 0 8px 8px; }
                        .footer { margin-top: 30px; padding-top: 20px; border-top: 1px solid #e0e0e0; font-size: 12px; color: #666; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='margin: 0; font-size: 24px;'>{{Title}}</h1>
                            <p style='margin: 10px 0 0 0; opacity: 0.9;'>{{ApplicationDisplayName}}</p>
                        </div>
                        <div class='content'>
                            <p><span class='severity-badge severity-{{toLowerCase Severity}}'>{{Severity}}</span></p>
                            <p><strong>Alert Type:</strong> {{AlertType}}</p>
                            <p><strong>Time:</strong> {{formatDate Timestamp 'yyyy-MM-dd HH:mm:ss'}}</p>
            
                            <div style='margin: 20px 0; padding: 20px; background: #f8f9fa; border-left: 4px solid {{severityColor Severity}}; border-radius: 4px;'>
                                {{Content}}
                            </div>
            
                            {{#if ActionUrl}}
                            <p style='margin-top: 30px;'>
                                <a href='{{ActionUrl}}' style='display: inline-block; padding: 12px 24px; background: #667eea; color: white; text-decoration: none; border-radius: 6px; font-weight: 600;'>Take Action</a>
                            </p>
                            {{/if}}
            
                            <div class='footer'>
                                <p>Notification ID: {{NotificationId}}</p>
                                <p>Environment: {{Environment}}</p>
                            </div>
                        </div>
                    </div>
                </body>
                </html>",
                                TextBodyTemplate = @"{{Title}}
                {{ApplicationDisplayName}}

                Severity: {{Severity}}
                Alert Type: {{AlertType}}
                Time: {{formatDate Timestamp 'yyyy-MM-dd HH:mm:ss'}}

                {{Content}}

                {{#if ActionUrl}}
                Take Action: {{ActionUrl}}
                {{/if}}

                Notification ID: {{NotificationId}}
                Environment: {{Environment}}"
            },

            // Security Alert Template
            new StarterTemplate
            {
                Key = "SECURITY_ALERT",
                Name = "Security Alert",
                Description = "Security incident notifications with threat details",
                Icon = "Material.Filled.Security",
                IconColor = "Error",
                Category = "Alerts",
                SupportedAlertTypes = new List<AlertType> { AlertType.Security },
                SubjectTemplate = "🔐 [SECURITY] {{Title}} - {{ApplicationDisplayName}}",
                HtmlBodyTemplate = @"<!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; line-height: 1.6; color: #333; }
                        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                        .header { background: linear-gradient(135deg, #dc3545 0%, #c82333 100%); color: white; padding: 30px; border-radius: 8px 8px 0 0; }
                        .security-icon { font-size: 48px; margin-bottom: 10px; }
                        .content { background: #ffffff; padding: 30px; border: 2px solid #dc3545; border-top: none; border-radius: 0 0 8px 8px; }
                        .alert-box { background: #f8d7da; border: 1px solid #f5c6cb; color: #721c24; padding: 15px; border-radius: 4px; margin: 20px 0; }
                        .details-grid { display: grid; grid-template-columns: 150px 1fr; gap: 10px; margin: 20px 0; }
                        .label { font-weight: 600; color: #666; }
                        .footer { margin-top: 30px; padding-top: 20px; border-top: 1px solid #e0e0e0; font-size: 12px; color: #666; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header' style='text-align: center;'>
                            <div class='security-icon'>🔐</div>
                            <h1 style='margin: 0; font-size: 24px;'>Security Alert</h1>
                            <p style='margin: 10px 0 0 0; opacity: 0.9;'>{{ApplicationDisplayName}}</p>
                        </div>
                        <div class='content'>
                            <div class='alert-box'>
                                <strong>⚠️ Security Event Detected</strong>
                                <p style='margin: 10px 0 0 0;'>{{Title}}</p>
                            </div>
            
                            <div class='details-grid'>
                                <div class='label'>Severity:</div>
                                <div><strong style='color: #dc3545;'>{{Severity}}</strong></div>
                
                                <div class='label'>Threat Level:</div>
                                <div>{{SecurityThreatLevel}}</div>
                
                                <div class='label'>Affected Resource:</div>
                                <div>{{AffectedResource}}</div>
                
                                <div class='label'>Detection Time:</div>
                                <div>{{formatDate Timestamp 'yyyy-MM-dd HH:mm:ss'}}</div>
                            </div>
            
                            <h3 style='color: #dc3545; margin-top: 30px;'>Description</h3>
                            <p>{{Content}}</p>
            
                            {{#if RemediationSteps}}
                            <h3 style='color: #dc3545; margin-top: 30px;'>Recommended Actions</h3>
                            <div style='background: #f8f9fa; padding: 20px; border-radius: 4px;'>
                                {{RemediationSteps}}
                            </div>
                            {{/if}}
            
                            {{#if ActionUrl}}
                            <p style='margin-top: 30px; text-align: center;'>
                                <a href='{{ActionUrl}}' style='display: inline-block; padding: 12px 30px; background: #dc3545; color: white; text-decoration: none; border-radius: 6px; font-weight: 600;'>Investigate Now</a>
                            </p>
                            {{/if}}
            
                            <div class='footer'>
                                <p><strong>Important:</strong> This is an automated security alert. Please investigate immediately.</p>
                                <p>Notification ID: {{NotificationId}} | Environment: {{Environment}}</p>
                            </div>
                        </div>
                    </div>
                </body>
                </html>",
                                TextBodyTemplate = @"SECURITY ALERT: {{Title}}
                {{ApplicationDisplayName}}

                Severity: {{Severity}}
                Threat Level: {{SecurityThreatLevel}}
                Affected Resource: {{AffectedResource}}
                Detection Time: {{formatDate Timestamp 'yyyy-MM-dd HH:mm:ss'}}

                Description:
                {{Content}}

                {{#if RemediationSteps}}
                Recommended Actions:
                {{RemediationSteps}}
                {{/if}}

                {{#if ActionUrl}}
                Investigate Now: {{ActionUrl}}
                {{/if}}

                This is an automated security alert. Please investigate immediately.
                Notification ID: {{NotificationId}} | Environment: {{Environment}}"
            },

            // Maintenance Notice Template
            new StarterTemplate
            {
                Key = "MAINTENANCE_NOTICE",
                Name = "Maintenance Notice",
                Description = "Scheduled maintenance and downtime notifications",
                Icon = "Material.Filled.BuildCircle",
                IconColor = "Warning",
                Category = "Notifications",
                SupportedAlertTypes = new List<AlertType> { AlertType.Maintenance },
                SubjectTemplate = "Scheduled Maintenance: {{ApplicationDisplayName}} - {{MaintenanceWindow}}",
                HtmlBodyTemplate = @"<!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; line-height: 1.6; color: #333; }
                        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                        .header { background: linear-gradient(135deg, #ffc107 0%, #ff9800 100%); color: #333; padding: 30px; border-radius: 8px 8px 0 0; text-align: center; }
                        .maintenance-icon { font-size: 48px; margin-bottom: 10px; }
                        .content { background: #ffffff; padding: 30px; border: 1px solid #ffc107; border-top: none; border-radius: 0 0 8px 8px; }
                        .info-box { background: #fff3cd; border: 1px solid #ffeaa7; padding: 20px; border-radius: 4px; margin: 20px 0; }
                        .timeline { margin: 30px 0; padding: 20px; background: #f8f9fa; border-radius: 8px; }
                        .footer { margin-top: 30px; padding-top: 20px; border-top: 1px solid #e0e0e0; font-size: 12px; color: #666; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <div class='maintenance-icon'>🔧</div>
                            <h1 style='margin: 0; font-size: 24px;'>Scheduled Maintenance</h1>
                            <p style='margin: 10px 0 0 0;'>{{ApplicationDisplayName}}</p>
                        </div>
                        <div class='content'>
                            <div class='info-box'>
                                <h3 style='margin: 0 0 10px 0;'>⚠️ Maintenance Window</h3>
                                <p style='margin: 0; font-size: 18px; font-weight: 600;'>{{MaintenanceWindow}}</p>
                                <p style='margin: 10px 0 0 0;'>Expected Duration: {{ExpectedDuration}}</p>
                            </div>
            
                            <h3>Maintenance Details</h3>
                            <p>{{Content}}</p>
            
                            <div class='timeline'>
                                <h4 style='margin: 0 0 15px 0;'>What to Expect:</h4>
                                <ul style='margin: 0; padding-left: 20px;'>
                                    <li>Service may be temporarily unavailable</li>
                                    <li>Performance degradation possible</li>
                                    <li>Brief interruptions during the maintenance window</li>
                                </ul>
                            </div>
            
                            {{#if AffectedServices}}
                            <h3>Affected Services</h3>
                            <p>{{AffectedServices}}</p>
                            {{/if}}
            
                            <p style='margin-top: 30px; padding: 20px; background: #e7f5ff; border-left: 4px solid #0d6efd; border-radius: 4px;'>
                                <strong>Note:</strong> We apologize for any inconvenience. Our team will work to minimize disruption and complete the maintenance as quickly as possible.
                            </p>
            
                            <div class='footer'>
                                <p>For urgent issues during maintenance, please contact support.</p>
                                <p>Notification ID: {{NotificationId}} | Environment: {{Environment}}</p>
                            </div>
                        </div>
                    </div>
                </body>
                </html>",
                                TextBodyTemplate = @"SCHEDULED MAINTENANCE NOTICE
                {{ApplicationDisplayName}}

                Maintenance Window: {{MaintenanceWindow}}
                Expected Duration: {{ExpectedDuration}}

                Maintenance Details:
                {{Content}}

                What to Expect:
                - Service may be temporarily unavailable
                - Performance degradation possible
                - Brief interruptions during the maintenance window

                {{#if AffectedServices}}
                Affected Services:
                {{AffectedServices}}
                {{/if}}

                We apologize for any inconvenience. Our team will work to minimize disruption.

                For urgent issues during maintenance, please contact support.
                Notification ID: {{NotificationId}} | Environment: {{Environment}}"
            },

            // Performance Alert Template
            new StarterTemplate
            {
                Key = "PERFORMANCE_ALERT",
                Name = "Performance Alert",
                Description = "Performance degradation and threshold breach notifications",
                Icon = "Material.Filled.Speed",
                IconColor = "Error",
                Category = "Alerts",
                SupportedAlertTypes = new List<AlertType> { AlertType.Performance },
                SubjectTemplate = "⚠️ Performance Alert: {{MetricName}} - {{ApplicationDisplayName}}",
                HtmlBodyTemplate = @"<!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; line-height: 1.6; color: #333; }
                        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                        .header { background: linear-gradient(135deg, #e74c3c 0%, #c0392b 100%); color: white; padding: 30px; border-radius: 8px 8px 0 0; }
                        .content { background: #ffffff; padding: 30px; border: 1px solid #e74c3c; border-top: none; border-radius: 0 0 8px 8px; }
                        .metric-card { background: #f8f9fa; border-left: 4px solid #e74c3c; padding: 20px; margin: 20px 0; border-radius: 4px; }
                        .metric-value { font-size: 36px; font-weight: bold; color: #e74c3c; }
                        .metric-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 20px; margin: 20px 0; }
                        .stat-box { background: #f8f9fa; padding: 15px; border-radius: 4px; text-align: center; }
                        .footer { margin-top: 30px; padding-top: 20px; border-top: 1px solid #e0e0e0; font-size: 12px; color: #666; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1 style='margin: 0; font-size: 24px;'>Performance Alert</h1>
                            <p style='margin: 10px 0 0 0; opacity: 0.9;'>{{ApplicationDisplayName}}</p>
                        </div>
                        <div class='content'>
                            <div class='metric-card'>
                                <h3 style='margin: 0 0 10px 0;'>{{MetricName}}</h3>
                                <div class='metric-value'>{{MetricValue}}</div>
                                <p style='margin: 10px 0 0 0; color: #666;'>Threshold: {{Threshold}}</p>
                            </div>
            
                            <div class='metric-grid'>
                                <div class='stat-box'>
                                    <strong>Severity</strong>
                                    <p style='margin: 5px 0 0 0; color: #e74c3c; font-weight: 600;'>{{Severity}}</p>
                                </div>
                                <div class='stat-box'>
                                    <strong>Duration</strong>
                                    <p style='margin: 5px 0 0 0; font-weight: 600;'>{{Duration}}</p>
                                </div>
                            </div>
            
                            <h3>Alert Details</h3>
                            <p>{{Content}}</p>
            
                            {{#if ImpactedUsers}}
                            <div style='margin: 20px 0; padding: 15px; background: #f8d7da; border-radius: 4px;'>
                                <strong>Impact:</strong> {{ImpactedUsers}} users affected
                            </div>
                            {{/if}}
            
                            {{#if ActionUrl}}
                            <p style='margin-top: 30px; text-align: center;'>
                                <a href='{{ActionUrl}}' style='display: inline-block; padding: 12px 30px; background: #e74c3c; color: white; text-decoration: none; border-radius: 6px; font-weight: 600;'>View Dashboard</a>
                            </p>
                            {{/if}}
            
                            <div class='footer'>
                                <p>Time: {{formatDate Timestamp 'yyyy-MM-dd HH:mm:ss'}}</p>
                                <p>Notification ID: {{NotificationId}} | Environment: {{Environment}}</p>
                            </div>
                        </div>
                    </div>
                </body>
                </html>",
                                TextBodyTemplate = @"PERFORMANCE ALERT: {{ApplicationDisplayName}}

                Metric: {{MetricName}}
                Current Value: {{MetricValue}}
                Threshold: {{Threshold}}

                Severity: {{Severity}}
                Duration: {{Duration}}

                Alert Details:
                {{Content}}

                {{#if ImpactedUsers}}
                Impact: {{ImpactedUsers}} users affected
                {{/if}}

                {{#if ActionUrl}}
                View Dashboard: {{ActionUrl}}
                {{/if}}

                Time: {{formatDate Timestamp 'yyyy-MM-dd HH:mm:ss'}}
                Notification ID: {{NotificationId}} | Environment: {{Environment}}"
            },

            // Health Check Alert Template
            new StarterTemplate
            {
                Key = "HEALTH_CHECK",
                Name = "Health Check Status",
                Description = "Service health check and status notifications",
                Icon = "Material.Filled.MonitorHeart",
                IconColor = "Success",
                Category = "Monitoring",
                SupportedAlertTypes = new List<AlertType> { AlertType.HealthCheck },
                SubjectTemplate = "Health Check {{Status}}: {{ApplicationDisplayName}}",
                HtmlBodyTemplate = @"<!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; line-height: 1.6; color: #333; }
                        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                        .header { padding: 30px; border-radius: 8px 8px 0 0; color: white; }
                        .header-success { background: linear-gradient(135deg, #28a745 0%, #218838 100%); }
                        .header-failure { background: linear-gradient(135deg, #dc3545 0%, #c82333 100%); }
                        .content { background: #ffffff; padding: 30px; border: 1px solid #e0e0e0; border-top: none; border-radius: 0 0 8px 8px; }
                        .status-icon { font-size: 48px; text-align: center; margin-bottom: 10px; }
                        .check-list { list-style: none; padding: 0; margin: 20px 0; }
                        .check-item { padding: 10px; margin: 5px 0; border-radius: 4px; display: flex; align-items: center; }
                        .check-pass { background: #d4edda; color: #155724; }
                        .check-fail { background: #f8d7da; color: #721c24; }
                        .footer { margin-top: 30px; padding-top: 20px; border-top: 1px solid #e0e0e0; font-size: 12px; color: #666; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header {{#if (eq Status ""Healthy"")}}header-success{{else}}header-failure{{/if}}'>
                            <div class='status-icon'>{{#if (eq Status ""Healthy"")}}✅{{else}}❌{{/if}}</div>
                            <h1 style='margin: 0; font-size: 24px; text-align: center;'>Health Check {{Status}}</h1>
                            <p style='margin: 10px 0 0 0; text-align: center; opacity: 0.9;'>{{ApplicationDisplayName}}</p>
                        </div>
                        <div class='content'>
                            <h3>Health Check Summary</h3>
                            <p>{{Content}}</p>
            
                            <ul class='check-list'>
                                {{#if DatabaseStatus}}
                                <li class='check-item {{#if (eq DatabaseStatus ""Healthy"")}}check-pass{{else}}check-fail{{/if}}'>
                                    <span style='margin-right: 10px;'>{{#if (eq DatabaseStatus ""Healthy"")}}✓{{else}}✗{{/if}}</span>
                                    Database Connection: {{DatabaseStatus}}
                                </li>
                                {{/if}}
                                {{#if ApiStatus}}
                                <li class='check-item {{#if (eq ApiStatus ""Healthy"")}}check-pass{{else}}check-fail{{/if}}'>
                                    <span style='margin-right: 10px;'>{{#if (eq ApiStatus ""Healthy"")}}✓{{else}}✗{{/if}}</span>
                                    API Endpoints: {{ApiStatus}}
                                </li>
                                {{/if}}
                                {{#if CacheStatus}}
                                <li class='check-item {{#if (eq CacheStatus ""Healthy"")}}check-pass{{else}}check-fail{{/if}}'>
                                    <span style='margin-right: 10px;'>{{#if (eq CacheStatus ""Healthy"")}}✓{{else}}✗{{/if}}</span>
                                    Cache Service: {{CacheStatus}}
                                </li>
                                {{/if}}
                            </ul>
            
                            <div style='margin: 20px 0; padding: 20px; background: #f8f9fa; border-radius: 4px;'>
                                <strong>Check Time:</strong> {{formatDate Timestamp 'yyyy-MM-dd HH:mm:ss'}}<br>
                                <strong>Response Time:</strong> {{ResponseTime}}ms<br>
                                <strong>Uptime:</strong> {{Uptime}}
                            </div>
            
                            {{#if ActionUrl}}
                            <p style='margin-top: 30px; text-align: center;'>
                                <a href='{{ActionUrl}}' style='display: inline-block; padding: 12px 30px; background: #0d6efd; color: white; text-decoration: none; border-radius: 6px; font-weight: 600;'>View Details</a>
                            </p>
                            {{/if}}
            
                            <div class='footer'>
                                <p>Environment: {{Environment}} | Next check scheduled in 5 minutes</p>
                                <p>Notification ID: {{NotificationId}}</p>
                            </div>
                        </div>
                    </div>
                </body>
                </html>",
                                TextBodyTemplate = @"HEALTH CHECK {{Status}}: {{ApplicationDisplayName}}

                Health Check Summary:
                {{Content}}

                Service Status:
                {{#if DatabaseStatus}}- Database Connection: {{DatabaseStatus}}{{/if}}
                {{#if ApiStatus}}- API Endpoints: {{ApiStatus}}{{/if}}
                {{#if CacheStatus}}- Cache Service: {{CacheStatus}}{{/if}}

                Check Time: {{formatDate Timestamp 'yyyy-MM-dd HH:mm:ss'}}
                Response Time: {{ResponseTime}}ms
                Uptime: {{Uptime}}

                {{#if ActionUrl}}
                View Details: {{ActionUrl}}
                {{/if}}

                Environment: {{Environment}}
                Next check scheduled in 5 minutes
                Notification ID: {{NotificationId}}"
            },

            // Welcome Email Template
            new StarterTemplate
            {
                Key = "WELCOME_EMAIL",
                Name = "Welcome Email",
                Description = "User onboarding and welcome notifications",
                Icon = "Material.Filled.Celebration",
                IconColor = "Success",
                Category = "User Communications",
                SupportedAlertTypes = new List<AlertType> { AlertType.System },
                SubjectTemplate = "Welcome to {{ApplicationDisplayName}}, {{UserDisplayName}}! 🎉",
                HtmlBodyTemplate = @"<!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; line-height: 1.6; color: #333; }
                        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
                        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 40px; border-radius: 8px 8px 0 0; text-align: center; }
                        .welcome-icon { font-size: 64px; margin-bottom: 20px; }
                        .content { background: #ffffff; padding: 40px; border: 1px solid #e0e0e0; border-top: none; border-radius: 0 0 8px 8px; }
                        .feature-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 20px; margin: 30px 0; }
                        .feature-box { padding: 20px; background: #f8f9fa; border-radius: 8px; text-align: center; }
                        .feature-icon { font-size: 36px; margin-bottom: 10px; }
                        .cta-button { display: inline-block; padding: 15px 40px; background: #667eea; color: white; text-decoration: none; border-radius: 50px; font-weight: 600; margin: 10px; }
                        .footer { margin-top: 40px; padding-top: 20px; border-top: 1px solid #e0e0e0; font-size: 12px; color: #666; text-align: center; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <div class='welcome-icon'>🎉</div>
                            <h1 style='margin: 0; font-size: 32px;'>Welcome to {{ApplicationDisplayName}}!</h1>
                            <p style='margin: 20px 0 0 0; font-size: 18px; opacity: 0.9;'>We're excited to have you on board, {{UserDisplayName}}</p>
                        </div>
                        <div class='content'>
                            <p style='font-size: 18px;'>Thank you for joining us! Your account has been successfully created and you're ready to get started.</p>
            
                            <div class='feature-grid'>
                                <div class='feature-box'>
                                    <div class='feature-icon'>📊</div>
                                    <h3 style='margin: 10px 0;'>Dashboard</h3>
                                    <p>Monitor your activities and track performance</p>
                                </div>
                                <div class='feature-box'>
                                    <div class='feature-icon'>⚙️</div>
                                    <h3 style='margin: 10px 0;'>Settings</h3>
                                    <p>Customize your experience and preferences</p>
                                </div>
                                <div class='feature-box'>
                                    <div class='feature-icon'>📚</div>
                                    <h3 style='margin: 10px 0;'>Resources</h3>
                                    <p>Access documentation and tutorials</p>
                                </div>
                                <div class='feature-box'>
                                    <div class='feature-icon'>💬</div>
                                    <h3 style='margin: 10px 0;'>Support</h3>
                                    <p>Get help when you need it</p>
                                </div>
                            </div>
            
                            <div style='text-align: center; margin: 40px 0;'>
                                <h2>Ready to Get Started?</h2>
                                <p>Here are some quick actions to help you begin:</p>
                                <p>
                                    <a href='{{ActionUrl}}' class='cta-button'>Access Dashboard</a>
                                </p>
                                <p>
                                    <a href='{{ProfileUrl}}' class='cta-button' style='background: #28a745;'>Complete Your Profile</a>
                                </p>
                            </div>
            
                            <div style='background: #e7f5ff; padding: 20px; border-radius: 8px; margin: 30px 0;'>
                                <h3 style='margin: 0 0 10px 0;'>🎁 Special Offer</h3>
                                <p>As a new member, you have access to exclusive features for your first 30 days. Make the most of your trial period!</p>
                            </div>
            
                            <div class='footer'>
                                <p>If you have any questions, don't hesitate to reach out to our support team.</p>
                                <p>{{ApplicationDisplayName}} © {{Year}}</p>
                            </div>
                        </div>
                    </div>
                </body>
                </html>",
                                TextBodyTemplate = @"Welcome to {{ApplicationDisplayName}}!

                Hi {{UserDisplayName}},

                We're excited to have you on board! Your account has been successfully created and you're ready to get started.

                What's Available to You:
                - Dashboard: Monitor your activities and track performance
                - Settings: Customize your experience and preferences
                - Resources: Access documentation and tutorials
                - Support: Get help when you need it

                Ready to Get Started?
                Access Dashboard: {{ActionUrl}}
                Complete Your Profile: {{ProfileUrl}}

                Special Offer:
                As a new member, you have access to exclusive features for your first 30 days. Make the most of your trial period!

                If you have any questions, don't hesitate to reach out to our support team.

                Best regards,
                The {{ApplicationDisplayName}} Team"
            },

            // Simple Notification Template
            new StarterTemplate
            {
                Key = "SIMPLE_NOTIFICATION",
                Name = "Simple Notification",
                Description = "Basic notification template for general use",
                Icon = "Material.Filled.Email",
                IconColor = "Primary",
                Category = "Basic",
                SupportedAlertTypes = new List<AlertType> { AlertType.System },
                SubjectTemplate = "{{Title}}",
                HtmlBodyTemplate = @"<!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; }
                        .container { max-width: 600px; margin: 0 auto; background: #ffffff; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden; }
                        .header { background: #f8f9fa; padding: 20px; border-bottom: 1px solid #e0e0e0; }
                        .content { padding: 30px; }
                        .footer { background: #f8f9fa; padding: 20px; border-top: 1px solid #e0e0e0; font-size: 12px; color: #666; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h2 style='margin: 0;'>{{Title}}</h2>
                            <p style='margin: 10px 0 0 0; color: #666;'>{{ApplicationDisplayName}}</p>
                        </div>
                        <div class='content'>
                            {{Content}}
            
                            {{#if ActionUrl}}
                            <p style='margin-top: 30px;'>
                                <a href='{{ActionUrl}}' style='display: inline-block; padding: 10px 20px; background: #0d6efd; color: white; text-decoration: none; border-radius: 4px;'>Take Action</a>
                            </p>
                            {{/if}}
                        </div>
                        <div class='footer'>
                            <p style='margin: 0;'>{{formatDate Timestamp 'yyyy-MM-dd HH:mm:ss'}} | {{Environment}}</p>
                        </div>
                    </div>
                </body>
                </html>",
                                TextBodyTemplate = @"{{Title}}
                {{ApplicationDisplayName}}

                {{Content}}

                {{#if ActionUrl}}
                Take Action: {{ActionUrl}}
                {{/if}}

                {{formatDate Timestamp 'yyyy-MM-dd HH:mm:ss'}} | {{Environment}}"
            }
        };

        public static StarterTemplate? GetTemplate(string key)
        {
            return Templates.Find(t => t.Key == key);
        }

        public static List<StarterTemplate> GetTemplatesByCategory(string category)
        {
            return Templates.FindAll(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        public static List<string> GetCategories()
        {
            return Templates.Select(t => t.Category).Distinct().ToList();
        }
    }
}