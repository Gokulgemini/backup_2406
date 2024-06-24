using System;

namespace RDM.Services.ImageVault
{
    public interface IDateTime
    {
        /// <summary>
        /// Gets the current date time.
        /// </summary>
        /// <returns>DateTime now</returns>
        DateTime Now();
    }
}
