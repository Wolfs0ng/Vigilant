using System;

namespace Vigilant.Exceptions
{
    /// <summary>
    /// Class for exceptions on event listeners
    /// </summary>
    public sealed class ListenerException : Exception
    {
        #region Methods

        public ListenerException(string msg)
            : base(msg)
        {
        }

        #endregion
    }
}