using System;

namespace Vigilant.Exceptions
{
    /// <summary>
    /// Class for exceptions on invalid generic types.
    /// </summary>
    public sealed class InvalidGenericTypeException : Exception
    {
        #region Methods

        public InvalidGenericTypeException(string msg)
            : base(msg)
        {
        }

        #endregion
    }
}