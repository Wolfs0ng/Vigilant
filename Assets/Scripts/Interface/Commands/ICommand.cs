using UnityEngine;

namespace Vigilant.Interface.Commands
{ 
    /// <summary>
    /// This interface will be used as a bridge between input and all tools (engine, weapon tower etc). 
    /// </summary>
    public interface ICommand
    {
        #region Properties
        #endregion

        #region Methods

        /// <summary>
        /// Call this when you want execute command
        /// </summary>
        void Execute();

        
        /// <summary>
        /// Call this when you want execute upgrade command
        /// </summary>
        void Upgrade();

        /// <summary>
        /// Call this when you want cancel command execution
        /// </summary>
        void Cancel();

        #endregion
    }
}