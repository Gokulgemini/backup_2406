using System;
using System.Threading.Tasks;
using Enyim.Caching;
using Microsoft.Extensions.Logging;
using RDM.Core;
using RDM.Data.ImageViewer;
using RDM.Data.ImageViewer.SqlServer;
using RDM.Model.Itms;

namespace RDM.Services.ImageViewerAPI
{
    public class PermissionsService : IPermissionService
    {
        private const int _memcachedCacheHoldTime = 15 * 60; // 15 minutes
        private readonly ILogger _logger;
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly IRolePermissionsRepository _rolePermissionsRepository;
        private readonly IMemcachedClient _memcachedClient;

        private class CachedPermission
        {
            public bool HasPermission { get; set; }
            public DateTimeOffset LastRefreshTime { get; set; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionsService"/> class
        /// with the supplied information.
        /// </summary>
        /// <param name="requestDataAccessor">The repository on which all service methods will operate.</param>
        /// <param name="logger">An instance of the system logger.</param>
        /// <param name="rolePermissionsRepository">An instance of the repository containing role permissions by userId. </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestDataAccessor"/> or <paramref name="rolePermissionsRepository"/> is <c>null</c>.
        /// </exception>
        public PermissionsService(
            IRequestDataAccessor requestDataAccessor,
            ILogger<PermissionsService> logger,
            IRolePermissionsRepository rolePermissionsRepository,
            IMemcachedClient memcachedClient)
        {
            Contract.Requires<ArgumentNullException>(requestDataAccessor != null, nameof(requestDataAccessor));
            Contract.Requires<ArgumentNullException>(rolePermissionsRepository != null, nameof(rolePermissionsRepository));
            Contract.Requires<ArgumentNullException>(logger != null, nameof(logger));
            Contract.Requires<ArgumentNullException>(memcachedClient != null, nameof(memcachedClient));

            _requestDataAccessor = requestDataAccessor;
            _rolePermissionsRepository = rolePermissionsRepository;
            _memcachedClient = memcachedClient;
            _logger = logger;
        }

        /// <summary>
        /// Checks if the given userId has permission to view mobile deposits.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="lastAuthenticationTimeUtc">The last time the user was authenticated.</param>
        /// <returns>true if the user has permission</returns>
        public async Task<bool> CanUpdateGeneralDocumentName(UserId userId, DateTime lastAuthenticationTimeUtc)
        {
            // check if we have anything in the cache
            var key = $"Permission_{userId}_{RolePermissionsRepository.UpdateNamePermission}";
            var cachedPermission = await _memcachedClient.GetValueAsync<CachedPermission>(key);

            // Compare the time the user was authenticated with the time the permission was pulled from the DB.
            // If the permission was pulled from the DB after the user was authenticated, the cached value is safe to use.
            if (cachedPermission == null || cachedPermission.LastRefreshTime.UtcDateTime < lastAuthenticationTimeUtc)
            {
                if (cachedPermission != null)
                {
                    _logger.LogDebug($"Stale cached permission for key {key}. Last Refresh Time: {cachedPermission.LastRefreshTime}, Last Authenticated: {lastAuthenticationTimeUtc}.");
                }
                else
                {
                    _logger.LogDebug($"Cache miss on key {key}.");
                }
                _logger.LogDebug($"Retrieving {RolePermissionsRepository.UpdateNamePermission} permission from the DB for user id {{UserId}}, RequestId: {{RequestId}}.", userId, _requestDataAccessor.RequestId);

                // get the permission from the database and cache the result
                cachedPermission = new CachedPermission()
                {
                    HasPermission = await _rolePermissionsRepository.CanUpdateGeneralDocumentName(userId),
                    LastRefreshTime = DateTimeOffset.Now
                };
                await _memcachedClient.SetAsync(key, cachedPermission, _memcachedCacheHoldTime);
            }

            return cachedPermission.HasPermission;
        }
    }
}