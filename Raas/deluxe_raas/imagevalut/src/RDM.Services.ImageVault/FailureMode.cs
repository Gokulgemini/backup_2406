namespace RDM.Services.ImageVault
{
    /// <summary>
    /// Controls the method by which the application behaves in the case of a fatal failure.
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// The method of failure handling could not be determined.
        /// </summary>
        Unknown,

        /// <summary>
        /// The application should report the fatal error and terminate.
        /// </summary>
        Stop,

        /// <summary>
        /// The application should report the fatal error and attempt to keep running.
        /// </summary>
        Ignore
    }

    public interface IFailureMode
    {
        Mode Mode { get; set; }
    }

    public class FailureMode : IFailureMode
    {
        public FailureMode(Mode mode)
        {
            Mode = mode;
        }

        public Mode Mode { get; set; }
    }
}