using System;
using System.Collections.Generic;
using RDM.Core.SqlServer;

namespace RDM.Data.ImageVault.SqlServer
{
    public class ImageVaultRepositoryInitializer : SqlServerRepositoryInitializer
    {
        public const int TargetRevision = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageVaultRepositoryInitializer"/> class
        /// with the supplied options.
        /// </summary>
        /// <param name="options">The settings used to control the behaviour of the initializer.</param>
        public ImageVaultRepositoryInitializer(IDictionary<string, string> options)
            : base(options)
        {
        }

        /// <inheritdoc/>
        protected override void CreateRepository(int schemaVersion)
        {
            CreateRepoSchema();
        }

        /// <inheritdoc/>
        protected override int GetTargetRevision()
        {
            return TargetRevision;
        }

        /// <inheritdoc/>
        protected override void UpgradeRepository(int fromVersion, int toVersion)
        {
            throw new NotImplementedException();
        }

        private void CreateRepoSchema()
        {
            // Image
            var query = @"
                CREATE TABLE Image
                (
                    ImageId NVARCHAR(32) NOT NULL,
                    MimeType NVARCHAR(30) NOT NULL,
                    Width INT NOT NULL,
                    Height INT NOT NULL,
                    ContentSize INT NOT NULL,
                    ExpiresOn DATETIMEOFFSET NOT NULL,
                    Content VARBINARY(MAX) NOT NULL,
                    CONSTRAINT pkImage PRIMARY KEY (ImageId)
                )
            ;";
            SqlServerRepository.ExecuteNonQuery(Db, query);

            query = @"
                ALTER PROCEDURE Purge
                AS
                BEGIN
                    SET NOCOUNT ON;

                    DELETE
                        Image
                    FROM
                        Image AS i
                    WHERE
                        i.ExpiresOn < SYSDATETIMEOFFSET();
                END
            ;";
            SqlServerRepository.ExecuteNonQuery(Db, query);
        }
    }
}
