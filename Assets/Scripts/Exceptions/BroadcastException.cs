using System;

namespace Vigilant.Exceptions
{
    /// <summary>
    /// Class for exceptions on event broadcast.
    /// </summary>
    public sealed class BroadcastException : Exception
    {
        #region Methods

        public BroadcastException(string msg)
            : base(msg)
        {
        }

        #endregion
    }
}