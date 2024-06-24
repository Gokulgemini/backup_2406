using System;
using RDM.Messaging;

namespace RDM.Services.ImageVault
{
    public class FatalErrorEventArgs : EventArgs
    {
        public FatalErrorEventArgs(IMessage message)
        {
            FailedMessage = message;
        }

        public IMessage FailedMessage { get; }
    }
}
