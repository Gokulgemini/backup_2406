using System;
using System.Collections.Generic;
using System.Linq;
using RDM.Core;
using RDM.Core.SqlServer;
using RDM.Model.Itms;
using Serilog;

namespace RDM.Data.ImageVault.SqlServer
{
    public class ImageRepository : SqlServerRepository, IImageRepository
    {
        private readonly IRequestDataAccessor _requestDataAccessor;
        private readonly IDictionary<string, string> _options;
        private readonly ILogger _logger;

        private const int DefaultImageRetentionDays = 3;
        private const string ImageRetentionDaysKey = "ImageRetentionDays";

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageRepository"/> class
        /// with the supplied options.
        /// </summary>
        /// <param name="requestDataAccessor">The request data accessor.</param>
        /// <param name="options">The settings used to control the behaviour of the repository.</param>
        /// <param name="logger">Logging utility.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="requestDataAccessor"/> is <c>null</c>.
        /// </exception>
        public ImageRepository(IRequestDataAccessor requestDataAccessor, IDictionary<string, string> options, ILogger logger)
            : base(options)
        {
            Contract.Requires<ArgumentNullException>(requestDataAccessor != null, nameof(requestDataAccessor));
            Contract.Requires<ArgumentNullException>(logger != null, nameof(logger));

            _requestDataAccessor = requestDataAccessor;
            _options = options;
            _logger = logger;
        }

        /// <inheritdoc/>
        public ImageId AddImage(byte[] content, string mimeType, int width, int height)
        {
            Contract.Requires<ArgumentNullException>(content != null, nameof(content));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(mimeType), nameof(mimeType));

            _requestDataAccessor.PerformanceMonitor?.Start("ImageRepository.AddImage");

            var retentionDays = DefaultImageRetentionDays;
            if (_options.TryGetValue(ImageRetentionDaysKey, out var value) &&
                int.TryParse(value, out var days))
            {
                retentionDays = days;
            }
            else
            {
                _logger.Warning($"Unable to obtain image retention days from configuration. The default value of '{retentionDays}' will be used. RequestId '{{RequestId}}'.",
                    _requestDataAccessor.RequestId);
            }

            var expiresOn = DateTimeOffset.Now.AddDays(retentionDays);

            using (Statistician.Factory.TraceLogger?.Tracer.BuildSpan("AddImage")
                .WithTag(OpenTracing.Tag.Tags.Component, "RDM.Data.ImageVault.SqlServer.ImageRepository")
                .StartActive())
            {
                var imageId = Guid.NewGuid().ToString("N");
                var query = @"
                    INSERT INTO Image
                    (
                        ImageId,
                        MimeType,
                        Width,
                        Height,
                        ContentSize,
                        ExpiresOn,
                        Content
                    )
                    VALUES
                    (
                        @imageId,
                        @mimeType,
                        @width,
                        @height,
                        @contentSize,
                        @expiresOn,
                        @content
                    )
                ;";

                var parameters = new Dictionary<string, object>
                {
                    { "@imageId", imageId },
                    { "@mimeType", mimeType },
                    { "@width", width },
                    { "@height", height },
                    { "@contentSize", content.Length },
                    { "@expiresOn", expiresOn },
                    { "@content", content }
                };

                ExecuteNonQuery(query, parameters);

                _requestDataAccessor.PerformanceMonitor?.Stop("ImageRepository.AddImage");

                return new ImageId(imageId);
            }
        }

        /// <inheritdoc/>
        public Result<Error, Image> GetImage(ImageId imageId)
        {
            Contract.Requires<ArgumentNullException>(imageId != null, nameof(imageId));
            _requestDataAccessor.PerformanceMonitor?.Start("ImageRepository.GetImage");

            using (Statistician.Factory.TraceLogger?.Tracer.BuildSpan("GetImage")
                .WithTag(OpenTracing.Tag.Tags.Component, "RDM.Data.ImageVault.SqlServer.ImageRepository")
                .StartActive())
            {
                var query = @"
                    SELECT
                        i.ImageId,
                        i.MimeType,
                        i.Width,
                        i.Height,
                        i.ContentSize,
                        i.ExpiresOn,
                        i.Content
                    FROM
                        Image AS i
                    WHERE
                        i.ImageId = @imageId
                ;";

                var parameters = new Dictionary<string, object>
                {
                    { "@imageId", imageId.ToString() }
                };

                var results = ExecuteReader(
                    query,
                    parameters,
                    reader =>
                    {
                        var mimeType = GetString(reader, "MimeType");
                        var width = GetInt(reader, "Width");
                        var height = GetInt(reader, "Height");
                        var contentSize = GetInt(reader, "ContentSize");
                        var content = new byte[contentSize];
                        reader.GetBytes(reader.GetOrdinal("Content"), 0, content, 0, contentSize);

                        return new Image(
                            content,
                            mimeType,
                            width,
                            height);
                    }).ToList();

                _requestDataAccessor.PerformanceMonitor?.Stop("ImageRepository.GetImage");

                return results.Any()
                    ? Result<Error, Image>.Success(results.First())
                    : Result<Error, Image>.Failure(new NotFound($"The requested image with image id '{imageId}' could not be found."));
            }
        }

        /// <inheritdoc/>
        public void UpdateImage(ImageId imageId, byte[] content, string mimeType, int width, int height)
        {
            Contract.Requires<ArgumentNullException>(imageId != null, nameof(imageId));
            Contract.Requires<ArgumentNullException>(content != null, nameof(content));
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(mimeType), nameof(mimeType));

            _requestDataAccessor.PerformanceMonitor?.Start("ImageRepository.UpdateImage");

            using (Statistician.Factory.TraceLogger?.Tracer.BuildSpan("UpdateImage")
                .WithTag(OpenTracing.Tag.Tags.Component, "RDM.Data.ImageVault.SqlServer.ImageRepository")
                .StartActive())
            {
                var query = @"
                    UPDATE Image
                    SET
                        MimeType = @mimeType,
                        Width = @width,
                        Height = @height,
                        ContentSize= @contentSize,
                        Content = @content
                    WHERE
                        ImageId = @imageId
                ;";

                var parameters = new Dictionary<string, object>
                {
                    { "@imageId", imageId.ToString() },
                    { "@mimeType", mimeType },
                    { "@width", width },
                    { "@height", height },
                    { "@contentSize", content.Length },
                    { "@content", content }
                };

                ExecuteNonQuery(query, parameters);

                _requestDataAccessor.PerformanceMonitor?.Stop("ImageRepository.UpdateImage");
            }
        }

        /// <inheritdoc/>
        public void RemoveImage(ImageId imageId)
        {
            Contract.Requires<ArgumentNullException>(imageId != null, nameof(imageId));

            _requestDataAccessor.PerformanceMonitor?.Start("ImageRepository.RemoveImage");

            using (Statistician.Factory.TraceLogger?.Tracer.BuildSpan("RemoveImage")
                .WithTag(OpenTracing.Tag.Tags.Component, "RDM.Data.ImageVault.SqlServer.ImageRepository")
                .StartActive())
            {
                var query = @"
                    DELETE
                    FROM
                        Image
                    WHERE
                        ImageId = @imageId
                ;";

                var parameters = new Dictionary<string, object>
                {
                    { "@imageId", imageId.ToString() }
                };

                ExecuteNonQuery(query, parameters);

                _requestDataAccessor.PerformanceMonitor?.Stop("ImageRepository.RemoveImage");
            }
        }
    }
}
