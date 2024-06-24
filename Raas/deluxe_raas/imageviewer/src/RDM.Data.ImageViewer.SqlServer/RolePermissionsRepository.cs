using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using RDM.Core;
using RDM.Model.Itms;

namespace RDM.Data.ImageViewer.SqlServer
{
    public class RolePermissionsRepository : IRolePermissionsRepository
    {
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly ILogger _logger;
        private readonly string _connectionString;

        // Permission(s) supported by this class
        public static readonly string UpdateNamePermission = "WCChangeGeneralDocumentName";

        public RolePermissionsRepository(
            IRequestDataAccessor requestDataAccessor,
            IDictionary<string, string> options,
            ILogger logger)
        {
            Contract.Requires<ArgumentNullException>(requestDataAccessor != null, nameof(requestDataAccessor));
            Contract.Requires<ArgumentNullException>(options != null, nameof(options));
            Contract.Requires<ArgumentNullException>(logger != null, nameof(logger));

            _requestDataAccessor = requestDataAccessor;
            _logger = logger;
            options.TryGetValue("ConnectionString", out _connectionString);
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(_connectionString), "ConnectionString");
        }

        /// <summary>
        /// Checks if the given userId has permission to view mobile deposits.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>true if userId has permission</returns>
        public async Task<bool> CanUpdateGeneralDocumentName(UserId userId)
        {
            _requestDataAccessor.PerformanceMonitor?.Start("ImageViewer.RolePermissionsRepository.CanUpdateGeneralDocumentName");

            try
            {
                var user = Convert.ToInt32(userId.Value);

                // Query to check if the role exists for the given userid
                var query = @"
                    SELECT 1 FROM
                        tblPermission AS p
                        INNER JOIN tblRolePermissions AS rp
                            INNER JOIN tblUserRoles AS ur
                                ON rp.fRoleID = ur.fRoleID
                            ON p.PermissionID = rp.fPermissionID
                    WHERE
                        ur.fUserID = @UserId
                        AND p.PermissionName = @RoleName
                ;";

                using (var connection = new SqlConnection(_connectionString))
                {
                    return await connection.QueryFirstOrDefaultAsync<bool>(
                        query,
                        new {UserId = user, RoleName = UpdateNamePermission});
                }
            }
            finally
            {
                _requestDataAccessor.PerformanceMonitor?.Stop("ImageViewer.RolePermissionsRepository.CanUpdateGeneralDocumentName");
            }
        }
    }
}
