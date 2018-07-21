using System;
using System.Collections.Generic;
using UnityEngine;
using Vigilant.Enums;

namespace Vigilant.Interface.Managers
{
    /// <summary>
    /// Base manager for all inputs type in game(keyboard,mouse+keyboard,gamepad,etc)
    /// </summary>
    public interface IInputManager
    {
        #region Fields

        /// <summary>
        /// Send when user change move Direction value.
        /// </summary>
        event Action<Vector2> MoveDirectionChanged;

        /// <summary>
        /// Send when user change aim Direction value.
        /// </summary>
        event Action<Vector2> WeaponTowerDirectionChanged;

        /// <summary>
        /// Press when user press on some button
        /// </summary>
        event Action<ButtonCode, ButtonState> Button;

//        /// <summary>
//        /// Occurs special use
//        /// </summary>
//        public virtual event Action<Vector2> SpecialUse = delegate { };
//     
// 
//     think about it. Special need direction ? yes. Always ? no. How to handle it ? ...
       
        #endregion

        #region Properties
        
        #endregion

        #region Unity Events
        #endregion

        #region Methods

        #endregion
    }
}