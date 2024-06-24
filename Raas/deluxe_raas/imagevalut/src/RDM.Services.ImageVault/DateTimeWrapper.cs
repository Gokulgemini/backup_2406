using System;
using System.Collections.Generic;
using System.Text;

namespace RDM.Services.ImageVault
{
    /// <summary>
    /// An implementation of the <see cref="IDateTime"/> interface that uses .NET's standard System DateTime functionality.
    /// </summary>
    public class DateTimeWrapper : IDateTime
    {
        /// <inheritdoc />
        public DateTime Now()
        {
            return DateTime.Now;
        }
    }
}
