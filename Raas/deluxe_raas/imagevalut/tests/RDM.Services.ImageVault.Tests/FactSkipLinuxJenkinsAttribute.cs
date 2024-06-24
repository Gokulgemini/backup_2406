using System;
using System.Runtime.InteropServices;
using Xunit;

namespace RDM.Services.ImageVault.Tests
{
    public sealed class FactSkipLinuxJenkinsAttribute : FactAttribute
    {
        public FactSkipLinuxJenkinsAttribute()
        {
            // Check if we're both on Windows *and* being run by jenkins
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && Environment.GetEnvironmentVariable("BUILD_ID") != null)
            {
                Skip = "Ignore on Jenkins in Linux";
            }
        }
    }
}
