using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using RDM.AspNetCore.Authentication.Itms;
using RDM.Middleware.ItmsApi;
using RDM.Model.Itms;

namespace RDM.Webservice.ImageViewerAPI
{
    /// <inheritdoc />
    /// <remarks>
    /// Provides a strongly typed interface to the HttpContext.Items dictionary.
    /// </remarks>
    public class RequestData : IRequestData
    {
        internal readonly HttpContext _context;

        private const string DefaultTenantValue = "Default";

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestData"/> class.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> to be wrapped.</param>
        public RequestData(HttpContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public UserId UserId
        {
            get =>
                _context.Items.Keys.Contains(AuthorizationMiddleware.UserIdKey)
                    ? new UserId((int)_context.Items[AuthorizationMiddleware.UserIdKey])
                    : default(UserId);

            set => _context.Items[AuthorizationMiddleware.UserIdKey] = int.Parse(value.Value);
        }

        /// <inheritdoc />
        public string ApplicationName
        {
            get
            {
                // Check to make sure they have a application name defined
                if (!_context.Request.Headers.Keys.Contains(AuthorizationMiddleware.ApplicationNameKey))
                {
                    return default(string);
                }

                // Now try to get the application name
                _context.Request.Headers.TryGetValue(AuthorizationMiddleware.ApplicationNameKey, out var values);

                return values.FirstOrDefault();
            }
        }

        /// <inheritdoc />
        public string ApplicationVersion
        {
            get
            {
                // Check to make sure they have a application name defined
                if (!_context.Request.Headers.Keys.Contains(AuthorizationMiddleware.ApplicationVersionKey))
                {
                    return default(string);
                }

                // Now try to get the application name
                _context.Request.Headers.TryGetValue(AuthorizationMiddleware.ApplicationVersionKey, out var values);

                return values.FirstOrDefault();
            }
        }

        /// <inheritdoc />
        public string TenantId
        {
            get
            {
                if (!_context.Items.Keys.Contains(AuthorizationMiddleware.TenantIdKey))
                {
                    return DefaultTenantValue;
                }

                var tenantIdValue = (string)_context.Items[AuthorizationMiddleware.TenantIdKey];
                return string.IsNullOrWhiteSpace(tenantIdValue) ? DefaultTenantValue : tenantIdValue;
            }
        }

        /// <inheritdoc />
        public RequestIdentifier RequestId =>
            !string.IsNullOrEmpty(_context.TraceIdentifier)
                ? new RequestIdentifier(_context.TraceIdentifier)
                : RequestIdentifier.Empty;

        /// <inheritdoc />
        public DateTime AuthenticationDateTimeUtc
        {
            get
            {
                if (_context.Items.ContainsKey(AuthorizationMiddleware.AuthenticationTimeUtc))
                {
                    return (DateTime)_context.Items[AuthorizationMiddleware.AuthenticationTimeUtc];
                }
                else
                {
                    var claim = _context.User.Claims?.FirstOrDefault(c => c.Type == ItmsAuthentication.ClaimTypeIds.AuthTime);
                    if (claim != null && long.TryParse(claim.Value, out var authTime))
                    {
                        return DateTimeOffset.FromUnixTimeSeconds(authTime).UtcDateTime;
                    }
                }

                return default;
            }
        }
    }
}
