using UnityEngine;

namespace Vigilant.Enums
{
    /// <summary>
    /// Codes of all buttons that user can choose
    /// </summary>
    public enum ButtonCode
    {
        #region Weapon buttons
        PrimaryFire,
        SecondaryFire,
        #endregion

        #region Special Abilities buttons
        Special,
        #endregion

        #region Brake buttons
        Handbrake,
        #endregion

        #region Camera buttons
        CameraChange,
        #endregion

        #region Game buttons
        Pause,
        #endregion

        #region Engine buttons
        StartEngine,
        #endregion

        #region Gear buttons
        GearShiftUp,
        GearShiftDown,
        #endregion
    }
}